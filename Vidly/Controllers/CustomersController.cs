﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using System.Data.Entity;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _contex;

        public CustomersController()
        {
            _contex = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _contex.Dispose();
        }


        public ActionResult New()
        {
            var membershipTypes = _contex.MembershipTypes.ToList();
            var viewModel = new CustomerFormViewModel()
            {
                MembershipTypes = membershipTypes,
            };
            return View("CustomerForm",viewModel);
        }

        [HttpPost] 
        public ActionResult Save(Customer customer)
        {
            if (customer.Id == 0)
            {
                _contex.Customers.Add(customer);
                _contex.SaveChanges();
                return RedirectToAction("Index", "Customers");
            }
            else
            {
                var customerInDb = _contex.Customers.Single(c => c.Id == customer.Id);

                //TryUpdateModel(customerInDb); MAYBE WILL BE SECURE PROBLEM
                //Mapper.Map(customer,customerInDb);//Use with Dto in dto you not update all values just which ones you need

                customerInDb.Name = customer.Name;
                customerInDb.Birthdate = customer.Birthdate;
                customerInDb.IsSubscribedToNewsletter = customer.IsSubscribedToNewsletter;
                customerInDb.MemberShipTypeId = customer.MemberShipTypeId;

            }

            _contex.SaveChanges();
            return RedirectToAction("Index", "Customers");
        }

        // GET: Customers
        public ActionResult Index()
        {
            var customers = _contex.Customers.Include(c=> c.MembershipType).ToList() ;
            return View(customers);
        }

        public ActionResult Details(int? id)
        {
            var customer = _contex.Customers.Include(c=>c.MembershipType).SingleOrDefault(c => c.Id == id);
            if (customer == null)
                return HttpNotFound();
            return View(customer);
        }

        public ActionResult Edit(int id)
        {
            var customer = _contex.Customers.SingleOrDefault(c => c.Id == id);

            if (customer == null)
                return HttpNotFound();
            var membershipTypes = _contex.MembershipTypes.ToList();

            var viewmodel = new CustomerFormViewModel()
            {
                Customer = customer,
                MembershipTypes = membershipTypes,
            };
            return View("CustomerForm", viewmodel);
        }
    }
}