using FluentMigrator;

namespace OutboxService.Infrastructure.Database.Migrations;

[Migration(20250513-1015)]
public class CreateTable_MyDataTable : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE IF NOT EXISTS MyDataTable (
    Id INT PRIMARY KEY GENERATED BY DEFAULT AS IDENTITY,
    Message TEXT NOT NULL,
    Timestamp TIMESTAMP NOT NULL    
)
");
    }

    public override void Down()
    {
        Execute.Sql(@"
DROP TABLE IF EXISTS MyDataTable
");
    }
}