using BlogSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BlogConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Body)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.HasOne(b => b.Author)
            .WithMany(a => a.Blogs)
            .HasForeignKey(b => b.AuthorId);
    }
}
