using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageSharingWithSecurity.Models
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id {get; init; }
        [MaxLength(20)]
        public virtual string Name {get; init;}

        public virtual ICollection<Image> Images {get; init;}
    }
}