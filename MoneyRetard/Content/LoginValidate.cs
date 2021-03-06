﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoneyRetard.Models;

namespace MoneyRetard.Content
{
    public class LoginValidate : AuthorizeAttribute
    {
        FlZlEntities fj = new FlZlEntities();
        public override void OnAuthorization(AuthorizationContext filterContext) {
            var routedata = filterContext.RouteData.Values.Values.ToArray();
            string controllername = routedata[0].ToString();
            if (controllername != "Welcome" && controllername != "Login" && controllername != "UserRegister") 
            {
                if (controllername == "Root") 
                {
                    if (HttpContext.Current.Session["limit"] == null && HttpContext.Current.Session["userID"] == null)
                    {
                        if (HttpContext.Current.Request.Cookies["ucookie"] != null)
                        {
                            int id = Convert.ToInt32(HttpContext.Current.Request.Cookies["ucookie"].Value);
                            FJ_User user = fj.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
                            HttpContext.Current.Session["userID"] = user.id;
                            HttpContext.Current.Session["limit"] = user.Limits;
                            if (user.Limits != 0)
                            {
                                string url = "Jump/Index";
                                HttpContext.Current.Response.Write("<script>window.parent.location.href=(\"../" + url + "\")</script>");
                            }
                        }
                        else
                        {
                            HttpContext.Current.Response.Write("<script>alert(\"请重新登陆~\");</script>");
                            HttpContext.Current.Response.Write("<script>window.parent.location.href=(\"../../Welcome/Index\")</script>");
                        }
                    }
                    else {
                        int id = Convert.ToInt32(HttpContext.Current.Session["userID"]);
                        FJ_User user = fj.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
                        HttpContext.Current.Session["userID"] = user.id;
                        HttpContext.Current.Session["limit"] = user.Limits;
                        if (user.Limits != 0)
                        {
                            string url = "Jump/Index";
                            HttpContext.Current.Response.Write("<script>window.parent.location.href=(\"../" + url + "\")</script>");
                        }
                    }
                }else{
                    var limit = HttpContext.Current.Session["limit"];
                    var userid = HttpContext.Current.Session["userID"];
                    if (HttpContext.Current.Session["limit"] == null && HttpContext.Current.Session["userID"] == null)
                    {
                        if (HttpContext.Current.Request.Cookies["ucookie"] == null)
                        {
                            HttpContext.Current.Response.Write("<script>alert(\"请重新登陆~\");</script>");
                            HttpContext.Current.Response.Write("<script>window.parent.location.href=(\"../../Welcome/Index\")</script>");
                        }
                        else
                        {
                            int id = Convert.ToInt32(HttpContext.Current.Request.Cookies["ucookie"].Value);
                            FJ_User user = fj.FJ_User.Where(u => u.id == id).ToList().FirstOrDefault();
                            HttpContext.Current.Session["userID"] = user.id;
                            HttpContext.Current.Session["limit"] = user.Limits;
                            if (user.Limits != 0)
                            {
                                string url = "Jump/Index";
                                HttpContext.Current.Response.Write("<script>window.parent.location.href=(\"../" + url + "\")</script>");
                            }
                        }
                    }
                }
            }
        }
    }
}