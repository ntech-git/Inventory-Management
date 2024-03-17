using Inventory_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Inventory_System.Controllers
{
    public class BuyerController : Controller
    {
        static string constr = @"data source=DESKTOP-JH55OB0;initial catalog=inventorySystem;integrated security=true";
        SqlConnection con = new SqlConnection(constr);
        [HttpGet]
        public ActionResult Home()
        {
            if (Session["email"] != null)
            {
                List<Products> productList = new List<Products>();

                con.Open();
                string query = "SELECT pid, name, image, description, price, category FROM product";
                SqlCommand cmd = new SqlCommand(query, con);

                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    Products product = new Products();
                    product.pid = Convert.ToInt32(sdr["pid"]);
                    product.name = sdr["name"].ToString();
                    product.file = sdr["image"].ToString();
                    product.description = sdr["description"].ToString();
                    product.price = Convert.ToInt32(sdr["price"]);
                    product.category = sdr["category"].ToString();

                    productList.Add(product);
                }
                con.Close();

                return View(productList);
            }
            else
            {
                return RedirectToAction("BuyerLogin");
            }
        }
        public ActionResult BuyerLogin()
        {
            HttpCookie uc = Request.Cookies["usercookie"];
            if (uc != null)
            {
                Buyer u = new Buyer();
                u.email = uc["email"].ToString();
                u.password = uc["password"].ToString();
                return View(u);
            }
            return View();
        }
        [HttpPost]
        public ActionResult BuyerLogin(Buyer b)
        {
            HttpCookie scookie = new HttpCookie("usercookie");
            scookie["email"] = b.email.ToString();
            scookie["password"] = b.password.ToString();
            Response.Cookies.Add(scookie);
            scookie.Expires = DateTime.Now.AddDays(7);

            con.Open();

            string query = "SELECT email, password, firstName, lastName FROM buyer where email='" + b.email + "' and password='" + b.password + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", b.email);
            cmd.Parameters.AddWithValue("@Password", b.password);
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                Session["firstName"] = sdr["firstName"].ToString();
                Session["lastName"] = sdr["lastName"].ToString();
                Session["email"] = sdr["email"].ToString();
                Session["password"] = sdr["password"].ToString();
            }
            else
            {
                Session["firstName"] = null;
                Session["lastName"] = null;
                Session["email"] = null;
                Session["password"] = null;
                Response.Write("<script>alert('" + "Invalid email or password. Please try again." + "'</script>");

            }

            sdr.Close();
            con.Close();

       
           return RedirectToAction("Home");
        }

        [HttpGet]
        public ActionResult BuyerSignup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BuyerSignup(Buyer b)
        {
            con.Open();
            var pfn = b.profilePicture.FileName;
            var ptext = Path.GetExtension(pfn);
            var allowext = new[] { ".png", ".jpg" };
            if (allowext.Contains(ptext))
            {
                var servefolderpath = Path.Combine(Server.MapPath("~/Content/images"), pfn);
                b.profilePicture.SaveAs(servefolderpath);
                var dbpath = "/images/" + pfn;
                string q = "INSERT INTO buyer (firstName, lastName, email, password, profilePicture) VALUES (@FirstName, @LastName, @Email, @Password, @DbPath)";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@FirstName", b.firstName);
                cmd.Parameters.AddWithValue("@LastName", b.lastName);
                cmd.Parameters.AddWithValue("@Email", b.email);
                cmd.Parameters.AddWithValue("@Password", b.password);
                cmd.Parameters.AddWithValue("@DbPath", dbpath);
                cmd.ExecuteNonQuery();
                Response.Write("<script>alert('" + "Signup Successfully" + "'</script>");
                con.Close();
            }

            con.Close();
            return RedirectToAction("BuyerLogin");
        }     
    }
}