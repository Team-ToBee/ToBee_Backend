using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToBee.API.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressReports",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalFocusTime = table.Column<double>(type: "float", nullable: false),
                    TotalTasksCompleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressReports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_ProgressReports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    RewardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    BadgeEarned = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateEarned = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.RewardId);
                    table.ForeignKey(
                        name: "FK_Rewards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskServices",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskServices", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskServices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AIRecommendations",
                columns: table => new
                {
                    RecommendationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Applied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRecommendations", x => x.RecommendationId);
                    table.ForeignKey(
                        name: "FK_AIRecommendations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIRecommendations_TaskServices_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskServices",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PomodoroSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BreakDuration = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PomodoroSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_PomodoroSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PomodoroSessions_TaskServices_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskServices",
                        principalColumn: "TaskId",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRecommendations_TaskId",
                table: "AIRecommendations",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AIRecommendations_UserId",
                table: "AIRecommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PomodoroSessions_TaskId",
                table: "PomodoroSessions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PomodoroSessions_UserId",
                table: "PomodoroSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressReports_UserId",
                table: "ProgressReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_UserId",
                table: "Rewards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskServices_UserId",
                table: "TaskServices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIRecommendations");

            migrationBuilder.DropTable(
                name: "PomodoroSessions");

            migrationBuilder.DropTable(
                name: "ProgressReports");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "TaskServices");
        }
    }
}
