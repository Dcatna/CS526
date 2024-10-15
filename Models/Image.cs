using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageSharingWithSecurity.Models
{
    public class Image
        /*
         * Entity model for an image.
         */
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; init; }
        [MaxLength(40)]
        public virtual string Caption { get; set; }
        [MaxLength(200)]
        public virtual string Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public virtual DateTime DateTaken { get; set; }

        [ForeignKey("User")]
        public virtual string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Tag")]
        public virtual int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        // The image has been validated.
        public virtual bool Valid { get; set; }

        // The image has been approved.
        public virtual bool Approved { get; set; }
    }
}