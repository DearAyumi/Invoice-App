﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    //Allows unauthenticated users to view Index tab.
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager) { }

        public IList<Invoice> Invoice { get;set; } = default!;

        //Displays only the invoices of the current User by comparing the user ID from database.
        //If the user has the "manager" role, show all invoices.
        public async Task OnGetAsync()
        {
            var invoices = from i in Context.Invoice select i;
            var isManager = User.IsInRole(Constants.InvoiceManagersRole);
            var isAdmin = User.IsInRole(Constants.InvoiceAdminRole);
            var currentUserId = UserManager.GetUserId(User);

            if (isManager == false && isAdmin == false) invoices = invoices.Where(i => i.CreatorId == currentUserId);

            Invoice = await invoices.ToListAsync();
        }
    }
}
