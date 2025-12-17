using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NizamDesk.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDatabaseDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ProjectId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "ProjectMemberships");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte[]>(
                name: "EntrySalt",
                table: "Companies",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "EntryPassword",
                table: "Companies",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ProjectId_Title",
                table: "Tickets",
                columns: new[] { "ProjectId", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMemberships_ProjectId",
                table: "ProjectMemberships",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_CompanyId",
                table: "CompanyMemberships",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMemberships_Companies_CompanyId",
                table: "CompanyMemberships",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyMemberships_Users_UserId",
                table: "CompanyMemberships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMemberships_Projects_ProjectId",
                table: "ProjectMemberships",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMemberships_Users_UserId",
                table: "ProjectMemberships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMemberships_Companies_CompanyId",
                table: "CompanyMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyMemberships_Users_UserId",
                table: "CompanyMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMemberships_Projects_ProjectId",
                table: "ProjectMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMemberships_Users_UserId",
                table: "ProjectMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ProjectId_Title",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_ProjectMemberships_ProjectId",
                table: "ProjectMemberships");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMemberships_CompanyId",
                table: "CompanyMemberships");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tickets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "ProjectMemberships",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "EntrySalt",
                table: "Companies",
                type: "bytea",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea");

            migrationBuilder.AlterColumn<byte[]>(
                name: "EntryPassword",
                table: "Companies",
                type: "bytea",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ProjectId",
                table: "Tickets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Companies_CompanyId",
                table: "Projects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
