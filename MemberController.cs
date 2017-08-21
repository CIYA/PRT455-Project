using DrwAutoMotors.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DrwAutoMotors.Controllers
{
    public class MemberController : Controller
    {
        private PRT405_DarwinAutoMotorsEntities db = new PRT405_DarwinAutoMotorsEntities();
        // GET: Member
        public ActionResult Index()
        {
            return RedirectToAction("CarListForSale");
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Login", "Home", new { area = "" });
        }

        public ActionResult CarListForSale()
        {
            return View(db.CarListForSales.ToList());
        }

        public ActionResult CarListForRent()
        {
            return View(db.RentalCarLists.ToList());
        }

        // GET: Catalogues/Delete/5
        public ActionResult AddToCartForRent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalCarList rentalcarlists = db.RentalCarLists.Find(id);

            if (rentalcarlists == null)
            {
                return HttpNotFound();
            }
            MyOrderForRent obj = new MyOrderForRent()
            {
                Make = rentalcarlists.Make,
                Model = rentalcarlists.Model,
                Price = rentalcarlists.Price,
                Year = rentalcarlists.Year,
                Quantity = rentalcarlists.Quantity,
                RentDuration = 0,
                RentFare = string.Empty,
                RentCarID = rentalcarlists.RentCarID,
                UserID = Convert.ToInt32(Session["UserID"])
            };
            return View(obj);
        }

        // POST: Catalogues/Delete/5
        [HttpPost, ActionName("AddToCartForRent")]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCartForRentConfirmed([Bind(Include = "MyOrderForRentID,Year,Make,Model,Price,Quantity,RentDuration,RentFare,RentCarID,UserID")] MyOrderForRent objMyOrderForRent)
        {
            if (ModelState.IsValid)
            {
                db.MyOrderForRents.Add(objMyOrderForRent);
                db.SaveChanges();

                RentalCarList objCarlistforRent = db.RentalCarLists.Find(objMyOrderForRent.RentCarID);
                if (objCarlistforRent != null)
                {
                    int quantity = Convert.ToInt32(objCarlistforRent.Quantity) - Convert.ToInt32(objMyOrderForRent.Quantity);
                    if (quantity > 0)
                    {
                        objCarlistforRent.Quantity = quantity;
                    }
                    else
                    {
                        objCarlistforRent.Quantity = 0;
                    }
                    db.Entry(objCarlistforRent).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("CarListForRent");
            }
            return View(objMyOrderForRent);
        }

        public ActionResult MyOrderForRent()
        {
            if (Session["UserID"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int id = Convert.ToInt32(Session["UserID"]);
            List<MyOrderForRent> lstMyOrderForRent = db.MyOrderForRents.Where(a => a.UserID == id).ToList();
            if (lstMyOrderForRent == null)
            {
                return HttpNotFound();
            }
            return View(lstMyOrderForRent);
        }

        // GET: Catalogues/Delete/5
        public ActionResult RemoveFromCartForRent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyOrderForRent objMyOrderForRent = db.MyOrderForRents.Find(id);
            if (objMyOrderForRent == null)
            {
                return HttpNotFound();
            }
            db.MyOrderForRents.Remove(objMyOrderForRent);
            db.SaveChanges();

            RentalCarList objRentalCarList = db.RentalCarLists.Find(objMyOrderForRent.RentCarID);
            if (objRentalCarList != null)
            {
                int quantity = Convert.ToInt32(objRentalCarList.Quantity) + Convert.ToInt32(objMyOrderForRent.Quantity);
                if (quantity > 0)
                {
                    objRentalCarList.Quantity = quantity;
                }
                else
                {
                    objRentalCarList.Quantity = 0;
                }
                db.Entry(objRentalCarList).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("MyOrderForRent");
        }

        public ActionResult AddToCartForSale(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CarListForSale carlistforsale = db.CarListForSales.Find(id);

            if (carlistforsale == null)
            {
                return HttpNotFound();
            }
            MyOrderForBuy obj = new MyOrderForBuy()
            {
                Make = carlistforsale.Make,
                Model = carlistforsale.Model,
                Price = carlistforsale.Price,
                Year = carlistforsale.Year,
                Quantity = carlistforsale.Quantity,
                SaleCarID = carlistforsale.CarID,
                UserID = Convert.ToInt32(Session["UserID"])
            };
            return View(obj);
        }


        // POST: Catalogues/Delete/5
        [HttpPost, ActionName("AddToCartForSale")]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCartForSaleConfirmed([Bind(Include = "MyOrderForBuyID,Year,Make,Model,Price,Quantity,UserID,SaleCarID")] MyOrderForBuy objMyOrderForBuy)
        {
            if (ModelState.IsValid)
            {
                db.MyOrderForBuys.Add(objMyOrderForBuy);
                db.SaveChanges();

                CarListForSale objCarlistforSale = db.CarListForSales.Find(objMyOrderForBuy.SaleCarID);
                if (objCarlistforSale != null)
                {
                    int quantity = Convert.ToInt32(objCarlistforSale.Quantity) - Convert.ToInt32(objMyOrderForBuy.Quantity);
                    if (quantity > 0)
                    {
                        objCarlistforSale.Quantity = quantity;
                    }
                    else
                    {
                        objCarlistforSale.Quantity = 0;
                    }
                    db.Entry(objCarlistforSale).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("CarListForSale");
            }
            return View(objMyOrderForBuy);
        }

        public ActionResult MyOrderForBuy()
        {
            if (Session["UserID"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int id = Convert.ToInt32(Session["UserID"]);
            List<MyOrderForBuy> lstMyOrderForBuy = db.MyOrderForBuys.Where(a => a.UserID == id).ToList();
            if (lstMyOrderForBuy == null)
            {
                return HttpNotFound();
            }
            return View(lstMyOrderForBuy);
        }

        // GET: Catalogues/Delete/5
        public ActionResult RemoveFromCartForBuy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MyOrderForBuy objMyOrderForBuy = db.MyOrderForBuys.Find(id);
            if (objMyOrderForBuy == null)
            {
                return HttpNotFound();
            }
            db.MyOrderForBuys.Remove(objMyOrderForBuy);
            db.SaveChanges();
            CarListForSale objCarListForSale = db.CarListForSales.Find(objMyOrderForBuy.SaleCarID);
            if (objCarListForSale != null)
            {
                int quantity = Convert.ToInt32(objCarListForSale.Quantity) + Convert.ToInt32(objMyOrderForBuy.Quantity);
                if (quantity > 0)
                {
                    objCarListForSale.Quantity = quantity;
                }
                else
                {
                    objCarListForSale.Quantity = 0;
                }
                db.Entry(objCarListForSale).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("MyOrderForBuy");
        }

        public ActionResult MyProfile()
        {
            User user = db.Users.Find(Convert.ToInt32(Session["UserID"]));
            return View(user);
        }

        [HttpPost]
        public ActionResult MyProfile([Bind(Include = "UserID,UserName,Email,Password,UserType,MobileNo,Address")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(user);
        }
    }
}