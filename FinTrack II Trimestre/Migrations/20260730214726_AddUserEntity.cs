using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTrack_II_Trimestre.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Users]') AND type = 'U')
                BEGIN
                    CREATE TABLE [Users] (
                        [id] int NOT NULL IDENTITY,
                        [name] nvarchar(20) NOT NULL,
                        [password] nvarchar(100) NOT NULL,
                        [status] bit NOT NULL,
                        [LoginAttempts] int NOT NULL,
                        CONSTRAINT [PK_Users] PRIMARY KEY ([id])
                    );
                    CREATE UNIQUE INDEX [IX_Users_name] ON [Users] ([name]);
                END
            """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[BudgetPlans]') AND type = 'U')
                BEGIN
                    CREATE TABLE [BudgetPlans] (
                        [PlanId] int NOT NULL IDENTITY,
                        [UserId] int NOT NULL,
                        [PlanType] nvarchar(50) NOT NULL,
                        [CreationDate] datetime2 NOT NULL,
                        [Status] bit NOT NULL,
                        CONSTRAINT [PK_BudgetPlans] PRIMARY KEY ([PlanId]),
                        CONSTRAINT [FK_BudgetPlans_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([id]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_BudgetPlans_UserId] ON [BudgetPlans] ([UserId]);
                END
            """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[PlanDetails]') AND type = 'U')
                BEGIN
                    CREATE TABLE [PlanDetails] (
                        [DetailId] int NOT NULL IDENTITY,
                        [PlanId] int NOT NULL,
                        [CategoryId] int NOT NULL,
                        CONSTRAINT [PK_PlanDetails] PRIMARY KEY ([DetailId]),
                        CONSTRAINT [FK_PlanDetails_BudgetPlans_PlanId] FOREIGN KEY ([PlanId]) REFERENCES [BudgetPlans] ([PlanId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_PlanDetails_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_PlanDetails_CategoryId] ON [PlanDetails] ([CategoryId]);
                    CREATE INDEX [IX_PlanDetails_PlanId] ON [PlanDetails] ([PlanId]);
                END
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanDetails");

            migrationBuilder.DropTable(
                name: "BudgetPlans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
