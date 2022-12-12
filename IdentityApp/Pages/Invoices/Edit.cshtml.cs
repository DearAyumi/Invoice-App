using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Authorization;

namespace IdentityApp.Pages.Invoices
{
    public class EditModel : DI_BasePageModel
    {

        public EditModel(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager) { }
        

        [BindProperty]
        public Invoice Invoice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Invoice == null)
            {
                return NotFound();
            }

            var invoice =  await Context.Invoice.FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }
            Invoice = invoice;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Update);
            if (isAuthorized.Succeeded == false) return Forbid();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            //Compares the ID taken from the Invoice table in database.
            var invoice = await Context.Invoice.AsNoTracking().SingleOrDefaultAsync(m => m.InvoiceId == id);           
            if(invoice == null)return NotFound();

            Invoice.CreatorId = invoice.CreatorId;
           
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Invoice, InvoiceOperations.Update);
            if (isAuthorized.Succeeded == false) return Forbid();

            //Prevents status changing from "rejected/approved" to "submitted" when changes are made by admin.
            Invoice.Status= invoice.Status;

            Context.Attach(Invoice).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(Invoice.InvoiceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool InvoiceExists(int id)
        {
          return Context.Invoice.Any(e => e.InvoiceId == id);
        }
    }
}
