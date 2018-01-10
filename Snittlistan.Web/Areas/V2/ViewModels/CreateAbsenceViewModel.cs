using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Raven.Abstractions;
using Snittlistan.Web.Areas.V2.Domain;

namespace Snittlistan.Web.Areas.V2.ViewModels
{
    public class CreateAbsenceViewModel : IValidatableObject
    {
        public CreateAbsenceViewModel()
        {
            From = To = SystemTime.UtcNow.Date;
            Comment = string.Empty;
        }

        public CreateAbsenceViewModel(Absence absence)
        {
            From = absence.From;
            To = absence.To;
            Player = absence.Player;
            Comment = absence.Comment;
        }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Player { get; set; }

        public string Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (To < SystemTime.UtcNow.Date)
                yield return new ValidationResult("Till-datum kan inte vara passerade datum");
            if (From > To)
                yield return new ValidationResult("Fr�n-datum kan inte vara f�re till-datum");
            if (From.Year != To.Year)
                yield return new ValidationResult("Fr�nvaro kan inte g� �ver flera �r");
            if (Comment != null && Comment.Length > 160)
                yield return new ValidationResult("Kommentar kan vara h�gst 160 tecken");
        }
    }
}