using BOTDiscord.DataBase.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BOTDiscord_RP.Migrations
{
    [DbContext(typeof(SDBContext))]
    [Migration("20210609022405_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.6");

            modelBuilder.Entity("BOTDiscord.DataBase.SQLite.Tables.SystemParams", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("System_Params");
                });
#pragma warning restore 612, 618
        }
    }
}
