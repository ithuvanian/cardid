﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Cardid.Models;
using Dapper;

namespace Cardid.DAL
{
    public class CardSqlDAL
    {
        private string connectionString;
        public CardSqlDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private string addCardToDeck = "INSERT INTO card_deck (CardID, DeckID) VALUES (@cardID, @deckID);";
        private string cardsNotWithDeck = "SELECT * FROM [cards] WHERE CardID NOT IN (SELECT CardID FROM [card_deck] "
            + "WHERE DeckID = @deckID) AND UserID = @userID";
        private string createCard = "INSERT INTO [cards] (Front, Back, UserID) VALUES (@front, @back, @userID);";
        private string editCard = "UPDATE [cards] SET Front = @front, Back = @back WHERE CardID = @cardID";
        private string getCardByID = "SELECT * FROM [cards] WHERE CardID = @cardID";
        private string getCardsByDeckID = "SELECT * FROM [cards] JOIN [card_deck] ON card_deck.CardID = cards.CardID "
            + "WHERE card_deck.DeckID = @deckID ORDER BY Front ASC";
        private string getCardsByUserID = "SELECT * FROM [cards] WHERE UserID = @userID ORDER BY Front ASC";
        private string removeDecksFromCard = "DELETE FROM [card_deck] WHERE CardID = @cardID";
        private string removeCard = "DELETE FROM [cards] WHERE CardID = @cardID";
        private string searchCardsForText = "SELECT * FROM cards WHERE Front LIKE @text OR Back LIKE @text";



        public void AddCardToDeck(Card card, string deckID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Execute(addCardToDeck, new { cardID = card.CardID, deckID });
            }
        }


        public List<Card> CardsNotWithDeck(Deck deck)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                List<Card> list = db.Query<Card>(cardsNotWithDeck, new { deckID = deck.DeckID, userID = deck.UserID }).ToList<Card>();
                foreach (Card card in list)
                {
                    card.TrimValues();
                }
                return list;
            }
        }


        public Card CreateCard(Card card, string userID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Execute(createCard, new { front = card.Front, back = card.Back, userID });
                List<Card> allCards = db.Query<Card>(getCardsByUserID, new { userID }).ToList<Card>();
                return allCards.Last().TrimValues();
            }
        }


        public void DeleteCard(string cardID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Execute(removeDecksFromCard, new { cardID });
                db.Execute(removeCard, new { cardID });
            }
        }


        public void EditCard(Card card)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                db.Execute(editCard, new { front = card.Front, back = card.Back, cardID = card.CardID });
            }
        }


        public Card GetCardByID(string cardID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Card>(getCardByID, new { cardID }).ToList().FirstOrDefault<Card>().TrimValues();
            }
        }


        public List<Card> GetCardsByDeckID(string deckID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                List<Card> list = db.Query<Card>(getCardsByDeckID, new { deckID }).ToList<Card>();
                foreach (Card card in list)
                {
                    card.TrimValues();
                }
                return list;
            }
        }


        public List<Card> GetCardsByUserID(string userID)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                List<Card> list = db.Query<Card>(getCardsByUserID, new { userID }).ToList<Card>();
                foreach (Card card in list)
                {
                    card.TrimValues();
                }
                return list;
            }
        }


        public List<Card> SearchCardsForText(string text)
        {
            using (SqlConnection db = new SqlConnection(connectionString))
            {
                List<Card> list = db.Query<Card>(searchCardsForText, new { text = "%" + text + "%" }).ToList<Card>();
                foreach (Card card in list)
                {
                    card.TrimValues();
                }
                return list;
            }
        }

    }
}