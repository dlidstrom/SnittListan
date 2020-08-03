namespace Snittlistan.Web.Areas.V2.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Snittlistan.Web.Areas.V2.Domain;

    public class EditMedalsPostModel : IValidatableObject
    {
        public EliteMedals.EliteMedal.EliteMedalValue? EliteMedal { get; set; }

        public int? CapturedSeason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EliteMedal == null)
            {
                yield return new ValidationResult("Medalj m�ste v�ljas");
            }
            else if (EliteMedal.Value != Domain.EliteMedals.EliteMedal.EliteMedalValue.None
                     && CapturedSeason.HasValue == false)
            {
                yield return new ValidationResult("S�song m�ste v�ljas");
            }
            else if (EliteMedal.Value == Domain.EliteMedals.EliteMedal.EliteMedalValue.None
                     && CapturedSeason.HasValue)
            {
                yield return new ValidationResult("S�song ska inte anges");
            }
        }
    }
}