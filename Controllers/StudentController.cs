using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.SqlClient;

using System.Net.Mail;
using System.Net;

namespace WebApplication1.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Index()
        {
            
            return View("Index");
        }


        [HttpPost]
        public ActionResult Index(login l) { 

            
            SqlConnection mycon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=prj1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            mycon.Open();
            try
            {
                //insertion des niveaux d'Education dans un ViewBag ===============================================================
                SqlCommand sqlCommand = new SqlCommand("Select * from etudiant " +
                    "where username = '"+l.username.Trim()+"' AND password = '" + l.password + "'", mycon);
                SqlDataReader myreader = sqlCommand.ExecuteReader();
                
                if (myreader.Read())
                {
                    if(myreader["statutConn"].ToString() == "0")
                    {
                        return View("FirstLogin"); // premiere connexion
                    }
                    else
                    {
                        transferData.username = l.username;
                        
                        return RedirectToAction("SecondLogin"); // redirect page des cours
                    }
                }
                else
                {
                    DataClasses1DataContext db = new DataClasses1DataContext();

                    var res = (from a in db.profs
                              where a.username == l.username && a.password == l.password
                              select a.IdProf).SingleOrDefault();

                    if(res != 0)
                    {
                        transferData.idProf = res;
                        return RedirectToAction("LogedIn", "Prof", null);
                    }
                    else
                    {
                        ViewBag.msg = "Mot de Passe ou Username est incorrecte !";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                mycon.Close();
            }
            
            return View("");
        }

        public ActionResult FirstLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FirstLogin(FirstLogin f)
        {
            SqlConnection mycon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=prj1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            mycon.Open();
            try
            {
                bool c1, c2,c3;
                c1 = c2 = c3 = false;
                string[] alp = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "K", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                string[] alp2 = { @"\", "|", "/", ",", "!", "@", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" };
                int[] alpNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                

                if (f.CnewMdp == f.newMdp)
                {
                    for (int i = 0; i < alp.Length; i++)
                    {
                        if (f.mdp.Contains(alp[i]))
                        {
                            c1 = true;
                        }
                    }

                    for (int i = 0; i < alp2.Length; i++)
                    {
                        if (f.mdp.Contains(alp2[i]))
                        {
                            c2 = true;
                        }
                    }

                    for (int i = 0; i < alpNum.Length; i++)
                    {
                        if (f.mdp.Contains(alpNum[i].ToString()))
                        {
                            c3 = true;
                        }
                    }

                    if(c1 == true && c2 == true && c3 == true)
                    {
                        SqlCommand sqlCommand = new SqlCommand("Update etudiant set password = '" + f.newMdp + "', StatutConn = 1 where password = '" + f.mdp + "'", mycon);
                        sqlCommand.ExecuteNonQuery();
                        return View("Index");
                    }
                    else if (c1 != true || c2 != true || c3 != true)
                    {
                        ViewBag.err = "Votre mot de passe doit contenir au moins un chiffre une lettre miniscule et majiscule et des characteres specifique @#$%^&*()";
                    }


                }  
                else
                {
                    ViewBag.err = "Password Not Comfirmed Correctly!";
                }
                            
            }
            catch(Exception ex)
            {
                ViewBag.err = "Oops! Something Went Wrong!";
            }
            finally
            {
                mycon.Close();
            }
            

            return View();
        }

        //SECOND LOGIN ===============================================================================================
        public ActionResult SecondLogin()
        {
            SqlConnection mycon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=prj1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            mycon.Open();
            try
            {
                string username = transferData.username;


                SqlCommand sqlcommand = new SqlCommand("select nivEduc from etudiant where username = '" + username + "'", mycon);
                SqlDataReader myreader = sqlcommand.ExecuteReader();

                if (myreader.Read())
                {
                    string nivEduc = myreader["nivEduc"].ToString();
                    myreader.Close();

                    SqlCommand sqlCommand1 = new SqlCommand("select * from cours where nivEduc = '" +nivEduc+ "'", mycon);
                    SqlDataReader myreader1 = sqlCommand1.ExecuteReader();
                    List<cours> lstC = new List<cours>();
                    if (myreader1.HasRows)
                    {
                        while (myreader1.Read())
                        {
                            cours c = new cours
                            {
                                idCours = Convert.ToInt32(myreader1["idCours"].ToString()),
                                nivEduc = myreader1["nivEduc"].ToString(),
                                nom = myreader1["nom"].ToString(),
                                desc = myreader1["desc"].ToString(),
                                image = myreader1["image"].ToString()
                            };

                            lstC.Add(c);

                        }

                        return View(lstC);
                    }
                    else
                    {
                        return View(lstC);
                    }
                    

                }
            }
            catch(Exception e)
            {

            }
            finally
            {
                mycon.Close();
            }
            return View();
        }

        [HttpPost]
        public ActionResult SecondLogin(String btnCour)
        {
            Int32 id = Convert.ToInt32(btnCour);

            transferData.idCours = id;



            return RedirectToAction("CourInfo");
        }


        public ActionResult Inscription()
        {       
            SqlConnection mycon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=prj1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            mycon.Open();
            try
            {
                //insertion des niveaux d'Education dans un ViewBag ===============================================================
                SqlCommand sqlCommand = new SqlCommand("Select * from nivEduc;", mycon);
                SqlDataReader myreader = sqlCommand.ExecuteReader();

                List<String> arrayEduc = new List<String>();



                while (myreader.Read())
                {
                    arrayEduc.Add(myreader["nivEduc"].ToString());
                }


                ViewBag.arrayEduc = arrayEduc;
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                mycon.Close();
            }

            return View("Inscription");
        }





        [HttpPost]
        public ActionResult Inscription(Inscription i)
        {
            // on click Submit button = (Inscriptoion i) => {

            SqlConnection mycon = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=prj1;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            mycon.Open();


            try
            {
                // verife si deja inscrit=========================================================================================
                SqlCommand sqlCommand = new SqlCommand("Select * from inscription where email = '" + i.email + "'", mycon);
                SqlDataReader myreader = sqlCommand.ExecuteReader();
                if (!myreader.Read())
                {
                    myreader.Close();
                    // insert table inscription ==================================================================================
                    SqlCommand sqlCommand1 = new SqlCommand("insert into inscription(nom, prenom, email, nivEduc)" +
                        " values('"+ i.nom +"', '"+ i.prenom +"', '"+i.email+"', '"+i.nivEduc+"')", mycon);
                    sqlCommand1.ExecuteNonQuery();

                    // generate password and username ============================================================================
                    string[] alp = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "K", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                    string[] alp2 = { @"\", "|", "/", ",", "!", "@", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" };
                    int[] alpNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
                    String mdp = "";

                    Random rnd = new Random();
                    for (int j = 0; j< 10; j++)
                    {
                        int num = rnd.Next(4);
                        if (num == 0)
                        {
                            Random Ralp = new Random();
                            mdp += alp[Ralp.Next(25)];
                        }
                        else if (num == 1)
                        {
                            Random Ralp = new Random();
                            mdp += alp2[Ralp.Next(18)];
                        }
                        else if (num == 2)
                        {
                            Random Ralp = new Random();
                            mdp += alpNum[Ralp.Next(9)];
                        }
                        else
                        {
                            Random Ralp = new Random();
                            mdp += alp[Ralp.Next(25)].ToUpper();
                        }
                    }


                    String username = DateTime.Today.Year.ToString() + i.nom.Substring(1, 3) + "-" + i.prenom.Substring(1, 3);

                    // insert into etudiant with password and username ===================================================================
                    SqlCommand sqlCommand2 = new SqlCommand("insert into etudiant(nom, prenom, email, nivEduc, username, password, statutConn)" +
                        " values('" + i.nom + "', '" + i.prenom + "', '" + i.email + "', '" + i.nivEduc + "', '"+username+"', '"+mdp+"', 0)"
                        , mycon);
                    sqlCommand2.ExecuteNonQuery();

                    Email mail = new Email
                    {
                        To = i.email,
                        Message = "Votre nouveau mot de passe est : '" + mdp + "'. Votre nouveau username est : '" + username + "'",
                        Subject = "Username et Mot de passe"
                    };

                    if (SendMail(mail))
                    {
                        ViewBag.msg = "le mail bien été send";
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.msg = "le mail n'a pas bien été send";
                        return View("Index");
                    }

                }
                else
                {
                    ViewBag.msg = "Vous etes deja etudiant!";
                    return View("Index", ViewBag.msg);
                }

            }
            catch(Exception ex)
            {

            }
            finally { mycon.Close(); }
                //        todo: 1 - verif si le client est deja un etudiant; Si oui ->PageLogin \ Si non -> Continue #2
                //        todo: 2 - generer un username et un password avec les infos recus
                //        todo: 3 - creer un nouveau compte etudiant-si Insert ok -> Continue #4
                //        todo: 4 - envoyer un email avec username, password et message de bienvenue
                /*        todo: 5-inviter l'etudiant a consulter son couriel*/

                return View();
        }



        public ActionResult CourInfo()
        {
            

            DataClasses1DataContext db = new DataClasses1DataContext();

            video res = (from videocours in db.videocours
                         join cours in db.cours
                         on videocours.IdCours equals cours.IdCours

                         join videos in db.videos
                         on videocours.IdVideo equals videos.IdVideo

                         where cours.IdCours == transferData.idCours

                         select videos).SingleOrDefault<video>();

            var res1 = (from cours in db.cours

                       where cours.IdCours == transferData.idCours

                       select cours.nom).SingleOrDefault<String>();

            ViewBag.res1 = res1.ToString();

            return View(res);
        }



        private bool SendMail(Email email)
        {

            string sender = System.Configuration.ConfigurationManager.AppSettings["mailSender"];
            string pw = System.Configuration.ConfigurationManager.AppSettings["mailPw"];

            try
            {
                SmtpClient smtpclient = new SmtpClient("smtp.office365.com", 587);
                smtpclient.Timeout = 3000;
                smtpclient.EnableSsl = true;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = new NetworkCredential(sender, pw);


                MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, email.To, email.Subject, email.Message);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpclient.Send(mailMessage);

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }

        }







    }


}