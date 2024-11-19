namespace KWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuloCitas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Citas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PacienteId = c.Int(nullable: false),
                        DoctorId = c.Int(nullable: false),
                        FechaHora = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Doctors", t => t.DoctorId, cascadeDelete: true)
                .ForeignKey("dbo.Pacientes", t => t.PacienteId, cascadeDelete: true)
                .Index(t => t.PacienteId)
                .Index(t => t.DoctorId);
            
            CreateTable(
                "dbo.Doctors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Especialidad = c.String(),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pacientes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Citas", "PacienteId", "dbo.Pacientes");
            DropForeignKey("dbo.Citas", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.Citas", new[] { "DoctorId" });
            DropIndex("dbo.Citas", new[] { "PacienteId" });
            DropTable("dbo.Pacientes");
            DropTable("dbo.Doctors");
            DropTable("dbo.Citas");
        }
    }
}
