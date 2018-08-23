﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cardid.DAL;
using Cardid.Models;

namespace Cardid.Controllers
{
    public class CardController : Controller
    {
        CardSqlDAL cardSql = new CardSqlDAL(ConfigurationManager.ConnectionStrings["FlashCardsDB"].ConnectionString);
        DeckSqlDAL deckSql = new DeckSqlDAL(ConfigurationManager.ConnectionStrings["FlashCardsDB"].ConnectionString);

        private string GetUser()
        {
            return Session["userid"].ToString();
        }

        private string GetBackground()
        {
            Background bg = new Background();
            return bg.Path();
        }


        public ActionResult Index()
        {
            GetUser();
            Session["background"] = GetBackground();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult EditCardInit(string cardID, string deckID, string searchString)
        {
            GetUser();

            Card card = cardSql.GetCardByID(cardID);

            if (deckID != null)
            {
                Session["background"] = GetBackground();
                card.CurrentDeckID = deckID;
            }
            else if (searchString != null)
            {
                Session["background"] = GetBackground();
                card.CurrentSearchString = searchString;
            }

            return View("EditCard", card);
        }


        [HttpPost]
        public ActionResult EditCardSubmit(Card newValues)
        {
            GetUser();

            cardSql.EditCard(newValues);

            return RedirectToAction("EditCardInit", new { cardID = newValues.CardID });
        }


        public ActionResult RemoveCard(string cardID, string deckID)
        {
            GetUser();

            cardSql.RemoveCard(cardID, deckID);

            TempData["card-removed"] = true;
            return RedirectToAction("EditDeck", "Deck", new { deckID });
        }

    }
}