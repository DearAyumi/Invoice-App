using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using IdentityApp.Models;

namespace IdentityApp.Authorization
{
    public class InvoiceCreatorAuthorizationHandler:
        AuthorizationHandler<OperationAuthorizationRequirement, Invoice>
    {
        UserManager<IdentityUser> _userManager;
        public InvoiceCreatorAuthorizationHandler(UserManager<IdentityUser> usermanager)
        {
            _userManager= usermanager; 
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Invoice invoice)       
        {
            if(context.User==null || invoice ==null) 
            { 
                return Task.CompletedTask; 
            }
            if(requirement.Name != Constants.CreateOperationName &&
               requirement.Name != Constants.ReadOperationName &&
               requirement.Name != Constants.UpdateOperationName &&
               requirement.Name != Constants.DeleteOperationName) 
            { 
                return Task.CompletedTask; 
            }
            //Compares if current user is the creator of the invoice using ID, authorization is valid if ID matches.
            if(invoice.CreatorId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
