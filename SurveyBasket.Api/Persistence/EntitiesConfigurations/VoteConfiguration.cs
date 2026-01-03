using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.Api.Entities;

namespace SurveyBasket.Api.Persistence.EntitiesConfigurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasIndex(x => new {x.PollId, x.UserId}).IsUnique();
 
        }
    }
}
