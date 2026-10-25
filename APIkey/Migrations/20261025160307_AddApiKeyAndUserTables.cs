using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIkey.Migrations
{
    /// <inheritdoc />
    public partial class AddApiKeyAndUserTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "api_keys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientIdentifier = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KeyHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Description = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Picture = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CategoryID);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    CustomerID = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    CompanyName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ContactName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ContactTitle = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Address = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    City = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Region = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    PostalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Country = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Phone = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Fax = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Image = table.Column<byte[]>(type: "longblob", nullable: true),
                    ImageThumbnail = table.Column<byte[]>(type: "longblob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CustomerID);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FirstName = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Title = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    TitleOfCourtesy = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    BirthDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Address = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    City = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Region = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    PostalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Country = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    HomePhone = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Extension = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Photo = table.Column<byte[]>(type: "longblob", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ReportsTo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "employees_ibfk_1",
                        column: x => x.ReportsTo,
                        principalTable: "employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "shippers",
                columns: table => new
                {
                    ShipperID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Phone = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ShipperID);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ContactName = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ContactTitle = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Address = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    City = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Region = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    PostalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Country = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Phone = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Fax = table.Column<string>(type: "varchar(24)", maxLength: 24, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    HomePage = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SupplierID);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RequiredDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShippedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ShipVia = table.Column<int>(type: "int", nullable: true),
                    Freight = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true, defaultValueSql: "'0.0000'"),
                    ShipName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ShipAddress = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ShipCity = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ShipRegion = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ShipPostalCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ShipCountry = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.OrderID);
                    table.ForeignKey(
                        name: "orders_ibfk_1",
                        column: x => x.CustomerID,
                        principalTable: "customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "orders_ibfk_2",
                        column: x => x.EmployeeID,
                        principalTable: "employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "orders_ibfk_3",
                        column: x => x.ShipVia,
                        principalTable: "shippers",
                        principalColumn: "ShipperID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProductName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    SupplierID = table.Column<int>(type: "int", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    QuantityPerUnit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true, defaultValueSql: "'0.0000'"),
                    UnitsInStock = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'0'"),
                    UnitsOnOrder = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'0'"),
                    ReorderLevel = table.Column<int>(type: "int", nullable: true, defaultValueSql: "'0'"),
                    Discontinued = table.Column<bool>(type: "tinyint(1)", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ProductID);
                    table.ForeignKey(
                        name: "products_ibfk_1",
                        column: x => x.SupplierID,
                        principalTable: "suppliers",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "products_ibfk_2",
                        column: x => x.CategoryID,
                        principalTable: "categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "order details",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValueSql: "'1'"),
                    Discount = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.OrderID, x.ProductID })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "order details_ibfk_1",
                        column: x => x.OrderID,
                        principalTable: "orders",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "order details_ibfk_2",
                        column: x => x.ProductID,
                        principalTable: "products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_api_keys_ClientIdentifier",
                table: "api_keys",
                column: "ClientIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_api_keys_IsActive",
                table: "api_keys",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "CategoryName",
                table: "categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "City",
                table: "customers",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "CompanyName",
                table: "customers",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "PostalCode",
                table: "customers",
                column: "PostalCode");

            migrationBuilder.CreateIndex(
                name: "Region",
                table: "customers",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "LastName",
                table: "employees",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "PostalCode1",
                table: "employees",
                column: "PostalCode");

            migrationBuilder.CreateIndex(
                name: "ReportsTo",
                table: "employees",
                column: "ReportsTo");

            migrationBuilder.CreateIndex(
                name: "OrderID",
                table: "order details",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "ProductID",
                table: "order details",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "CustomerID",
                table: "orders",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "EmployeeID",
                table: "orders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "OrderDate",
                table: "orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "ShippedDate",
                table: "orders",
                column: "ShippedDate");

            migrationBuilder.CreateIndex(
                name: "ShipPostalCode",
                table: "orders",
                column: "ShipPostalCode");

            migrationBuilder.CreateIndex(
                name: "ShipVia",
                table: "orders",
                column: "ShipVia");

            migrationBuilder.CreateIndex(
                name: "CategoryID",
                table: "products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "ProductName",
                table: "products",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "SupplierID",
                table: "products",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "CompanyName1",
                table: "suppliers",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "PostalCode2",
                table: "suppliers",
                column: "PostalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_keys");

            migrationBuilder.DropTable(
                name: "order details");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "shippers");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
