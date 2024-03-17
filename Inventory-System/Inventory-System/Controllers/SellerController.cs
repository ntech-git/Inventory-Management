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
    public class SellerController : Controller
    {
        static string constr = @"data source=DESKTOP-JH55OB0;initial catalog=inventorySystem;integrated security=true";
        SqlConnection con = new SqlConnection(constr);
        [HttpGet]
        public ActionResult Home()
        {
            if (Session["firstName"] != null && Session["email"] != null)
            {
                return View();
            }
            return RedirectToAction("SellerLogin");
        }
        public ActionResult SellerLogin()
        {
            HttpCookie uc = Request.Cookies["usercookie"];
            if (uc != null)
            {
                Seller u = new Seller();
                u.email = uc["email"].ToString();
                u.password = uc["password"].ToString();
                return View(u);
            }
            return View();
        }
        [HttpPost]
        public ActionResult SellerLogin(Seller s)
        {
            HttpCookie scookie = new HttpCookie("usercookie");
            scookie["email"] = s.email.ToString();
            scookie["password"] = s.password.ToString();
            Response.Cookies.Add(scookie);
            scookie.Expires = DateTime.Now.AddDays(7);

            con.Open();

            string query = "SELECT sid,email, password, firstName, lastName,profilePicture FROM [seller] where email='" + s.email + "' and password='" + s.password + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", s.email);
            cmd.Parameters.AddWithValue("@Password", s.password);
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                Session["sid"] = sdr["sid"].ToString();
                Session["firstName"] = sdr["firstName"].ToString();
                Session["lastName"] = sdr["lastName"].ToString();
                Session["email"] = sdr["email"].ToString();
                Session["password"] = sdr["password"].ToString();
                Session["profilePicture"] = sdr["profilePicture"].ToString();
            }
            else
            {
                Session["firstName"] = null;
                Session["lastName"] = null;
                Session["email"] = null;
                Session["password"] = null;
                Session["profilePicture"] = null;
                Response.Write("<script>alert('" + "Invalid email or password. Please try again." + "'</script>");

            }

            sdr.Close();
            con.Close();


            return RedirectToAction("Home");
        }

        [HttpGet]
        public ActionResult SellerSignup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SellerSignup(Seller s)
        {
            con.Open();
            var pfn = s.profilePicture.FileName;
            var ptext = Path.GetExtension(pfn);
            var allowext = new[] { ".png", ".jpg" };
            if (allowext.Contains(ptext))
            {
                var servefolderpath = Path.Combine(Server.MapPath("~/Content/images"), pfn);
                s.profilePicture.SaveAs(servefolderpath);
                var dbpath = "/images/" + pfn;
                string q = "INSERT INTO [seller] (firstName, lastName, email, password, profilePicture) VALUES (@FirstName, @LastName, @Email, @Password, @DbPath)";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@FirstName", s.firstName);
                cmd.Parameters.AddWithValue("@LastName", s.lastName);
                cmd.Parameters.AddWithValue("@Email", s.email);
                cmd.Parameters.AddWithValue("@Password", s.password);
                cmd.Parameters.AddWithValue("@DbPath", dbpath);
                cmd.ExecuteNonQuery();
                Response.Write("<script>alert('" + "Signup Successfully" + "'</script>");
                con.Close();
            }

            con.Close();
            return RedirectToAction("SellerLogin");
        }
        [HttpGet]
        public ActionResult AllProducts()
        {
            if (Session["sid"] != null && Session["email"] != null)
            {
                List<Products> productList = new List<Products>();

                con.Open();
                string query = "SELECT pid, name, image, description, price, category FROM product WHERE sid = @Sid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Sid", Session["sid"].ToString());

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
                return RedirectToAction("SellerLogin");
            }
        }
        [HttpPost]
        public ActionResult AllProducts(string category)
        {
            if (Session["sid"] != null && Session["email"] != null)
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    // If category is empty, redirect to the GET action method for retrieving all products
                    return RedirectToAction("AllProducts");
                }
                else
                {
                    List<Products> productList = new List<Products>();

                con.Open();
                string query = "SELECT pid, name, image, description, price, category FROM product WHERE sid = @Sid AND category = @Category";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Sid", Session["sid"].ToString());
                cmd.Parameters.AddWithValue("@Category", category);
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
            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }
        [HttpGet]
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Products s)
        {
            con.Open();
            var pfn = s.image.FileName;
            var ptext = Path.GetExtension(pfn);
            var allowext = new[] { ".png", ".jpg" };
            if (allowext.Contains(ptext))
            {
                var servefolderpath = Path.Combine(Server.MapPath("~/images"), pfn);
                s.image.SaveAs(servefolderpath);
                var dbpath = "/images/" + pfn;
                string q = "INSERT INTO [product] (name, image, description, price,sid, category) VALUES (@Name,@DbPath, @Description, @Price,@Sid, @Category)";
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@Name", s.name);
                cmd.Parameters.AddWithValue("@DbPath", dbpath);
                cmd.Parameters.AddWithValue("@Description", s.description);
                cmd.Parameters.AddWithValue("@Price", s.price);
                cmd.Parameters.AddWithValue("@Sid", Session["sid"].ToString());
                cmd.Parameters.AddWithValue("@Category", s.category);
                cmd.ExecuteNonQuery();
                Response.Write("<script>alert('" + "Signup Successfully" + "'</script>");
                con.Close();
            }

            con.Close();
            return RedirectToAction("AllProducts");
        }
        //For Delete
        [HttpGet]
        public ActionResult DeleteProduct()
        {
            try
            {
                // Extract product ID from query parameters
                int pid;
                if (!int.TryParse(Request.Params["pid"], out pid))
                {
                    // Handle case where pid is not provided or not a valid integer
                    TempData["ErrorMessage"] = "Invalid product ID";
                    return RedirectToAction("AllProducts");
                }

                con.Open();
                string query = "DELETE FROM product WHERE pid = @Pid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Pid", pid);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Product deleted successfully
                    return RedirectToAction("AllProducts");
                }
                else
                {
                    // Product with given pid not found
                    TempData["ErrorMessage"] = "Product not found";
                    return RedirectToAction("AllProducts");
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                TempData["ErrorMessage"] = "An error occurred while deleting the product: " + ex.Message;
                return RedirectToAction("AllProducts");
            }
            finally
            {
                con.Close();
            }
        }
        [HttpGet]
        public ActionResult EditProduct(int pid)
        {
            con.Open();
            string query = "Select * from product where pid='" + pid + "'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Read();
            Products a = new Products();
            a.pid = int.Parse(sdr["pid"].ToString());
            a.name = sdr["name"].ToString();
            a.description = sdr["description"].ToString();
            a.price = int.Parse(sdr["price"].ToString());
            a.category = sdr["category"].ToString();
            sdr.Close();
            con.Close();

            return View(a);

        }
        [HttpPost]
        public ActionResult EditProduct(Products a)
        {
            try
            {
                con.Open();
                string query = "UPDATE product SET name = @Name, description = @Description, price = @Price, category = @Category WHERE pid = @Pid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Name", a.name);
                cmd.Parameters.AddWithValue("@Description", a.description);
                cmd.Parameters.AddWithValue("@Price", a.price);
                cmd.Parameters.AddWithValue("@Category", a.category);
                cmd.Parameters.AddWithValue("@Pid", a.pid);
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("AllProducts");
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                TempData["ErrorMessage"] = "An error occurred while editing the product: " + ex.Message;
                return RedirectToAction("AllProducts");
            }
            finally
            {
                con.Close();
            }
        }


    }
}