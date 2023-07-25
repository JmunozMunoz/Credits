using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sc.Credits.AppServices.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AuthMethodsSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "CollectTypesSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "ParamtersSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "PaymentStatusesSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "PaymentTypesSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "ProfilesSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "SourcesSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "StatusSeq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "TransactionTypesSeq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "AuthMethods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    EffectiveAnnualRate = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    AssurancePercentage = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    MandatoryDownPayment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LastNumber = table.Column<long>(nullable: false),
                    StoreId = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    StoreName = table.Column<string>(maxLength: 200, nullable: false),
                    CollectTypeId = table.Column<int>(nullable: true),
                    AssuranceType = table.Column<int>(nullable: false),
                    BusinessGroupId = table.Column<string>(maxLength: 50, nullable: false),
                    DownPaymentPercentage = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    MandatoryDownPayment = table.Column<bool>(nullable: false),
                    MinimumFee = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    VendorId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_CollectTypes_CollectTypeId",
                        column: x => x.CollectTypeId,
                        principalTable: "CollectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 50, nullable: false),
                    IdDocument = table.Column<string>(maxLength: 50, nullable: false),
                    DocumentType = table.Column<string>(maxLength: 20, nullable: false),
                    ProfileId = table.Column<int>(nullable: false),
                    AvailableCreditLimit = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreateTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    Defaulter = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    FullName = table.Column<string>(maxLength: 200, nullable: false),
                    Mobile = table.Column<string>(maxLength: 20, nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "date", nullable: false),
                    UpdateTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ValidatedMail = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreditsMaster",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreateTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LastDate = table.Column<DateTime>(type: "date", nullable: false),
                    LastTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LastId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    CertifiedId = table.Column<string>(maxLength: 200, nullable: true),
                    CertifyingAuthority = table.Column<string>(maxLength: 200, nullable: true),
                    CreditDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreditNumber = table.Column<long>(nullable: false),
                    CreditTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Invoice = table.Column<string>(maxLength: 200, nullable: true),
                    Products = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Reason = table.Column<string>(maxLength: 1000, nullable: true),
                    ScCode = table.Column<string>(maxLength: 20, nullable: false),
                    Seller = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditsMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditsMaster_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditsMaster_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditsMaster_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ArrearsValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    AssuranceValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    BankAccount = table.Column<string>(nullable: true),
                    CalculationDate = table.Column<DateTime>(type: "date", nullable: false),
                    ChargeValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    CreditValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    Fee = table.Column<int>(nullable: false),
                    InterestValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "date", nullable: false),
                    LastPaymentTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PaymentTypeId = table.Column<int>(nullable: true),
                    PaymentTypeName = table.Column<string>(maxLength: 200, nullable: false),
                    TotalValuePaid = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    TransactionTypeId = table.Column<int>(nullable: true),
                    TransactionTypeName = table.Column<string>(maxLength: 200, nullable: false),
                    StoreId = table.Column<string>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    AuthMethodId = table.Column<int>(nullable: false),
                    AlternatePayment = table.Column<bool>(nullable: false),
                    ArrearsCharge = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    ArrearsChargeBalance = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    ArrearsDays = table.Column<int>(nullable: true),
                    AssuranceFee = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    AssurancePercentage = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    AssuranceValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    AuthMethodName = table.Column<string>(maxLength: 200, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    ChargeBalance = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    ChargeValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    ComputedArrearsDays = table.Column<int>(nullable: false, computedColumnSql: "(CASE WHEN DATEDIFF(DAY,[DueDate],GETDATE()) <= 0 THEN 0 ELSE DATEDIFF(DAY,[DueDate],GETDATE()) END)"),
                    CreateDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreateTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreditMasterId = table.Column<Guid>(nullable: true),
                    CreditNumber = table.Column<long>(nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    FeeValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    Fees = table.Column<int>(nullable: false),
                    Frequency = table.Column<int>(nullable: false),
                    HasUpdatedPaymentPlan = table.Column<bool>(nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(26, 6)", nullable: false),
                    LastFee = table.Column<int>(nullable: false),
                    Location = table.Column<string>(maxLength: 50, nullable: false),
                    Products = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    SourceName = table.Column<string>(maxLength: 200, nullable: false),
                    StatusName = table.Column<string>(maxLength: 200, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "date", nullable: false),
                    TransactionTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedPaymentPlanBalance = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    UpdatedPaymentPlanValue = table.Column<decimal>(type: "decimal(26, 6)", nullable: true),
                    UserId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Credits_AuthMethods_AuthMethodId",
                        column: x => x.AuthMethodId,
                        principalTable: "AuthMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credits_CreditsMaster_CreditMasterId",
                        column: x => x.CreditMasterId,
                        principalTable: "CreditsMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Credits_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credits_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credits_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credits_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credits_PaymentTypes_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Credits_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalTable: "TransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreditId = table.Column<Guid>(nullable: false),
                    CancelDate = table.Column<DateTime>(nullable: true),
                    CancelTime = table.Column<TimeSpan>(nullable: true),
                    CreditCancelId = table.Column<Guid>(nullable: true),
                    CreditMasterId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    PaymentStatusId = table.Column<int>(nullable: true),
                    PaymentStatusName = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    StoreId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_PaymentStatuses_PaymentStatusId",
                        column: x => x.PaymentStatusId,
                        principalTable: "PaymentStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentDetails_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credits_AuthMethodId",
                table: "Credits",
                column: "AuthMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_CreditMasterId",
                table: "Credits",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_CustomerId",
                table: "Credits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_SourceId",
                table: "Credits",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_StatusId",
                table: "Credits",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_StoreId",
                table: "Credits",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_PaymentTypeId",
                table: "Credits",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Credits_TransactionTypeId",
                table: "Credits",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_CustomerId",
                table: "CreditsMaster",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_LastId",
                table: "CreditsMaster",
                column: "LastId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_StatusId",
                table: "CreditsMaster",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_StoreId",
                table: "CreditsMaster",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditsMaster_CreditNumber_StoreId",
                table: "CreditsMaster",
                columns: new[] { "CreditNumber", "StoreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ProfileId",
                table: "Customers",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IdDocument_DocumentType",
                table: "Customers",
                columns: new[] { "IdDocument", "DocumentType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditCancelId",
                table: "PaymentDetails",
                column: "CreditCancelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditId",
                table: "PaymentDetails",
                column: "CreditId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CreditMasterId",
                table: "PaymentDetails",
                column: "CreditMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_CustomerId",
                table: "PaymentDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_PaymentStatusId",
                table: "PaymentDetails",
                column: "PaymentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetails_StoreId",
                table: "PaymentDetails",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Sequences_StoreId_Type",
                table: "Sequences",
                columns: new[] { "StoreId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BusinessGroupId",
                table: "Stores",
                column: "BusinessGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CollectTypeId",
                table: "Stores",
                column: "CollectTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "PaymentDetails");

            migrationBuilder.DropTable(
                name: "Sequences");

            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "PaymentStatuses");

            migrationBuilder.DropTable(
                name: "AuthMethods");

            migrationBuilder.DropTable(
                name: "CreditsMaster");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "CollectTypes");

            migrationBuilder.DropSequence(
                name: "AuthMethodsSeq");

            migrationBuilder.DropSequence(
                name: "CollectTypesSeq");

            migrationBuilder.DropSequence(
                name: "ParamtersSeq");

            migrationBuilder.DropSequence(
                name: "PaymentStatusesSeq");

            migrationBuilder.DropSequence(
                name: "PaymentTypesSeq");

            migrationBuilder.DropSequence(
                name: "ProfilesSeq");

            migrationBuilder.DropSequence(
                name: "SourcesSeq");

            migrationBuilder.DropSequence(
                name: "StatusSeq");

            migrationBuilder.DropSequence(
                name: "TransactionTypesSeq");
        }
    }
}