using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CodeConnect.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace CodeConnect.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser() : base()
    {
        Chats = new List<ChatUser>();
    }

    [PersonalData]
    [Column(TypeName = "nvarchar(50)")]
    public string ChosenUserName {  get; set; }

    public ICollection<ChatUser> Chats { get; set; }
}
