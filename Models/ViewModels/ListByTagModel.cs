using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImageSharingWithSecurity.Models.ViewModels
{
    public class ListByTagModel
    {
        public int Id { get; init; }
        public IEnumerable<SelectListItem> Tags { get; init; }
    }
}
