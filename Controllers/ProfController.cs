using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1;

namespace WebApplication1.Controllers
{
    public class ProfController : Controller
    {
        // GET: Prof
        public ActionResult LogedIn()
        {
            int x = transferData.idProf;

            DataClasses1DataContext db = new DataClasses1DataContext();

            List<cour> res = (from cours in db.cours
                       join prof in db.profs
                       on cours.IdProf equals prof.IdProf

                       where cours.IdProf == x
                       select cours).ToList();

            

            String res1 = (from prof in db.profs
                       where prof.IdProf == x
                       select prof.username).SingleOrDefault();

            ViewBag.res = res1;

            return View(res);
           
        }



        [HttpPost]
        public ActionResult LogedIn(int IdCours)
        {


            return View("AccessCour", IdCours);

        }



        public ActionResult AccessCour(/*int IdCours*/)
        {
        //    int x = transferData.idProf;
        //    int IdCour = IdCours;

        //    DataClasses1DataContext db = new DataClasses1DataContext();

            return View();
        }
    }
}