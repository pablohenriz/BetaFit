using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BetaFit.Infrastructure.Migrations;

[Migration("20260819193000_AddStoreTables")]
public partial class AddStoreTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name:"CartItems", columns:table=>new { Id=table.Column<Guid>(nullable:false), UserId=table.Column<Guid>(nullable:false), ProductId=table.Column<Guid>(nullable:false), Quantity=table.Column<int>(nullable:false), CreatedAt=table.Column<DateTime>(nullable:false), UpdatedAt=table.Column<DateTime>(nullable:true), IsDeleted=table.Column<bool>(nullable:false) }, constraints:table=>{table.PrimaryKey("PK_CartItems",x=>x.Id);table.ForeignKey("FK_CartItems_Products_ProductId",x=>x.ProductId,"Products","Id",onDelete:ReferentialAction.Restrict);table.ForeignKey("FK_CartItems_Users_UserId",x=>x.UserId,"Users","Id",onDelete:ReferentialAction.Cascade);});
        migrationBuilder.CreateTable(name:"Orders", columns:table=>new { Id=table.Column<Guid>(nullable:false), UserId=table.Column<Guid>(nullable:false), Total=table.Column<decimal>(type:"decimal(18,2)",nullable:false), Status=table.Column<string>(maxLength:40,nullable:false), CreatedAt=table.Column<DateTime>(nullable:false), UpdatedAt=table.Column<DateTime>(nullable:true), IsDeleted=table.Column<bool>(nullable:false) }, constraints:table=>{table.PrimaryKey("PK_Orders",x=>x.Id);table.ForeignKey("FK_Orders_Users_UserId",x=>x.UserId,"Users","Id",onDelete:ReferentialAction.Restrict);});
        migrationBuilder.CreateTable(name:"OrderItems", columns:table=>new { Id=table.Column<Guid>(nullable:false), OrderId=table.Column<Guid>(nullable:false), ProductId=table.Column<Guid>(nullable:false), ProductName=table.Column<string>(maxLength:150,nullable:false), UnitPrice=table.Column<decimal>(type:"decimal(18,2)",nullable:false), Quantity=table.Column<int>(nullable:false), CreatedAt=table.Column<DateTime>(nullable:false), UpdatedAt=table.Column<DateTime>(nullable:true), IsDeleted=table.Column<bool>(nullable:false) }, constraints:table=>{table.PrimaryKey("PK_OrderItems",x=>x.Id);table.ForeignKey("FK_OrderItems_Orders_OrderId",x=>x.OrderId,"Orders","Id",onDelete:ReferentialAction.Cascade);});
        migrationBuilder.CreateIndex(name:"IX_CartItems_ProductId",table:"CartItems",column:"ProductId");
        migrationBuilder.CreateIndex(name:"IX_CartItems_UserId_ProductId",table:"CartItems",columns:new[]{"UserId","ProductId"},unique:true);
        migrationBuilder.CreateIndex(name:"IX_OrderItems_OrderId",table:"OrderItems",column:"OrderId");
        migrationBuilder.CreateIndex(name:"IX_Orders_UserId",table:"Orders",column:"UserId");
    }
    protected override void Down(MigrationBuilder migrationBuilder){migrationBuilder.DropTable(name:"CartItems");migrationBuilder.DropTable(name:"OrderItems");migrationBuilder.DropTable(name:"Orders");}
}
