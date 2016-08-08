using MDM.WebPortal.Models.FromDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MDM.WebPortal.Models.FromDB
{
    public class ActionCodeDictRepository
    {
        //private MedProDBEntities db;

        //static data - FOR TEST PURPOSES ONLY
        private static List<ActionCodeDict> orderList = new List<ActionCodeDict>
        {
            //new Order {OrderID = 1, OrderDate = new DateTime(2012,8,1), EmployeeID = 1, OrderDescription = "Order Food", IsCompleted = true, OrderTotal = 70.20M},
            //new Order {OrderID = 2, OrderDate = new DateTime(2012,8,1), EmployeeID = 2, OrderDescription = "Order Office Materials", IsCompleted = true, OrderTotal = 99.75M },
            //new Order {OrderID = 3, OrderDate = new DateTime(2012,8,2), EmployeeID = 1, OrderDescription = "Order Production Materials", IsCompleted = false, OrderTotal = 24500.00M },
            //new Order {OrderID = 4, OrderDate = new DateTime(2012,8,3), EmployeeID = 4, OrderDescription = "Order Food", IsCompleted = true, OrderTotal = 29.99M }
        };

        public static IList<ActionCodeDict> GetAll()
        {

            IList<ActionCodeDict> result = (IList<ActionCodeDict>)HttpContext.Current.Session["ActionCodeDicts"];

            if (result == null)
            {
                var db = new MedProDBEntities();
                result = db.ActionCodeDicts.ToList();


                //read from dataBase
                //HttpContext.Current.Session["Orders"] = result =
                //    (from order in orderList
                //     select new Order
                //     {
                //         OrderID = order.OrderID,
                //         OrderDate = order.OrderDate,
                //         EmployeeID = order.EmployeeID,
                //         OrderDescription = order.OrderDescription,
                //         IsCompleted = order.IsCompleted,
                //         OrderTotal = order.OrderTotal
                //     }).ToList();
            }

            return result;
        }

        public static ActionCodeDict One(Func<ActionCodeDict, bool> predicate)
        {
            return GetAll().Where(predicate).FirstOrDefault();
        }

        //public static void Insert(ActionCodeDict order)
        //{
        //    order.id = GetAll().OrderByDescending(p => p.id).First().id + 1;

        //    GetAll().Insert(0, order);
        //}

        public static void Update(ActionCodeDict order)
        {
            ActionCodeDict target = One(o => o.id == order.id);
            if (target != null)
            {
                using (var db = new MedProDBEntities())
                {

                    target.id = order.id;
                    target.CollNoteType = order.CollNoteType;
                    target.Code = order.Code;
                    target.CategoryID = order.CategoryID;
                    target.Priority = order.Priority;
                    target.NTUser = order.NTUser;
                    target.Active = order.Active;
                    target.ParsingYN = order.ParsingYN;

                    db.ActionCodeDicts.Attach(order);
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }

            }
        }

        //public static void Remove(ActionCodeDict order)
        //{
        //    ActionCodeDict target = One(o => o.id == order.id);
        //    if (target != null)
        //    {
        //        GetAll().Remove(target);
        //    }
        //}
    }
}