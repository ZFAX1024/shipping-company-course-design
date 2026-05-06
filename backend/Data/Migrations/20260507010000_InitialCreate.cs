using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ShippingCompany.Api.Data;

namespace ShippingCompany.Api.Data.Migrations;

[DbContext(typeof(ShippingDbContext))]
[Migration("20260507010000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                ContactName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Address = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ShippingRoutes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RouteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                OriginPort = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DestinationPort = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                DistanceNm = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                EstimatedDays = table.Column<int>(type: "int", nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShippingRoutes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Role = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Vessels",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ImoNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CapacityTons = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                CurrentPort = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vessels", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Cargoes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                WeightTons = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                VolumeCbm = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Hazardous = table.Column<bool>(type: "bit", nullable: false),
                Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cargoes", x => x.Id);
                table.ForeignKey(
                    name: "FK_Cargoes_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TransportOrders",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
                CargoId = table.Column<int>(type: "int", nullable: false),
                VesselId = table.Column<int>(type: "int", nullable: true),
                ShippingRouteId = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                Progress = table.Column<int>(type: "int", nullable: false),
                PlannedDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                PlannedArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                ActualDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                ActualArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                FreightAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TransportOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_TransportOrders_Cargoes_CargoId",
                    column: x => x.CargoId,
                    principalTable: "Cargoes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TransportOrders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TransportOrders_ShippingRoutes_ShippingRouteId",
                    column: x => x.ShippingRouteId,
                    principalTable: "ShippingRoutes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TransportOrders_Vessels_VesselId",
                    column: x => x.VesselId,
                    principalTable: "Vessels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "FinanceSettlements",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                SettlementNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                TransportOrderId = table.Column<int>(type: "int", nullable: false),
                ReceivableAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FinanceSettlements", x => x.Id);
                table.ForeignKey(
                    name: "FK_FinanceSettlements_TransportOrders_TransportOrderId",
                    column: x => x.TransportOrderId,
                    principalTable: "TransportOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "PaymentRecords",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FinanceSettlementId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                PaymentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                TransactionNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PaymentRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_PaymentRecords_FinanceSettlements_FinanceSettlementId",
                    column: x => x.FinanceSettlementId,
                    principalTable: "FinanceSettlements",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Cargoes_CustomerId", table: "Cargoes", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_FinanceSettlements_SettlementNo", table: "FinanceSettlements", column: "SettlementNo", unique: true);
        migrationBuilder.CreateIndex(name: "IX_FinanceSettlements_TransportOrderId", table: "FinanceSettlements", column: "TransportOrderId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_PaymentRecords_FinanceSettlementId", table: "PaymentRecords", column: "FinanceSettlementId");
        migrationBuilder.CreateIndex(name: "IX_ShippingRoutes_RouteCode", table: "ShippingRoutes", column: "RouteCode", unique: true);
        migrationBuilder.CreateIndex(name: "IX_TransportOrders_CargoId", table: "TransportOrders", column: "CargoId");
        migrationBuilder.CreateIndex(name: "IX_TransportOrders_CustomerId", table: "TransportOrders", column: "CustomerId");
        migrationBuilder.CreateIndex(name: "IX_TransportOrders_OrderNo", table: "TransportOrders", column: "OrderNo", unique: true);
        migrationBuilder.CreateIndex(name: "IX_TransportOrders_ShippingRouteId", table: "TransportOrders", column: "ShippingRouteId");
        migrationBuilder.CreateIndex(name: "IX_TransportOrders_VesselId", table: "TransportOrders", column: "VesselId");
        migrationBuilder.CreateIndex(name: "IX_Users_UserName", table: "Users", column: "UserName", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Vessels_ImoNumber", table: "Vessels", column: "ImoNumber", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PaymentRecords");
        migrationBuilder.DropTable(name: "Users");
        migrationBuilder.DropTable(name: "FinanceSettlements");
        migrationBuilder.DropTable(name: "TransportOrders");
        migrationBuilder.DropTable(name: "Cargoes");
        migrationBuilder.DropTable(name: "ShippingRoutes");
        migrationBuilder.DropTable(name: "Vessels");
        migrationBuilder.DropTable(name: "Customers");
    }
}
