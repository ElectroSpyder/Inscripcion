using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ADJInsc.Models.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barrios",
                columns: table => new
                {
                    bardep = table.Column<double>(nullable: true),
                    barloc = table.Column<double>(nullable: true),
                    barbar = table.Column<double>(nullable: true),
                    bardes = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Departamento",
                columns: table => new
                {
                    DepartamentoKey = table.Column<int>(nullable: false),
                    DepartamentoDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Departam__A31840122D895C9B", x => x.DepartamentoKey);
                });

            migrationBuilder.CreateTable(
                name: "DirDupli",
                columns: table => new
                {
                    ins_id = table.Column<int>(nullable: false),
                    cantidad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Direccion",
                columns: table => new
                {
                    FLIFIC = table.Column<double>(nullable: true),
                    DOMCAL = table.Column<string>(maxLength: 255, nullable: true),
                    DOMPUE = table.Column<double>(nullable: true),
                    DOMPIS = table.Column<string>(maxLength: 255, nullable: true),
                    DOMDEP = table.Column<string>(maxLength: 255, nullable: true),
                    DOMBAR = table.Column<double>(nullable: true),
                    DOMLOT = table.Column<string>(maxLength: 255, nullable: true),
                    DOMMZA = table.Column<string>(maxLength: 255, nullable: true),
                    DOMDTO = table.Column<double>(nullable: true),
                    DOMLOC = table.Column<double>(nullable: true),
                    DOMREF = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Familia1",
                columns: table => new
                {
                    FLIFIC = table.Column<double>(nullable: true),
                    FLISFI = table.Column<double>(nullable: true),
                    FLICFI = table.Column<double>(nullable: true),
                    FLIFIN = table.Column<double>(nullable: true),
                    FLIAPN = table.Column<string>(maxLength: 255, nullable: true),
                    FLITDO = table.Column<double>(nullable: true),
                    FLINDO = table.Column<double>(nullable: true),
                    FLINAC = table.Column<double>(nullable: true),
                    FLIFNA = table.Column<double>(nullable: true),
                    FLILOC = table.Column<double>(nullable: true),
                    FLIPRO = table.Column<double>(nullable: true),
                    FLIPAI = table.Column<double>(nullable: true),
                    FLIECI = table.Column<double>(nullable: true),
                    FLISEX = table.Column<double>(nullable: true),
                    FLIDEP = table.Column<double>(nullable: true),
                    FLILCM = table.Column<double>(nullable: true),
                    FLIDIS = table.Column<double>(nullable: true),
                    FLICOM = table.Column<double>(nullable: true),
                    FLITEF = table.Column<string>(maxLength: 255, nullable: true),
                    FLICUI = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Familia2",
                columns: table => new
                {
                    FLIFIC = table.Column<double>(nullable: true),
                    FLISFI = table.Column<double>(nullable: true),
                    FLICFI = table.Column<double>(nullable: true),
                    FLIFIN = table.Column<double>(nullable: true),
                    FLIAPN = table.Column<string>(maxLength: 255, nullable: true),
                    FLITDO = table.Column<double>(nullable: true),
                    FLINDO = table.Column<int>(nullable: true),
                    FLINAC = table.Column<double>(nullable: true),
                    FLIFNA = table.Column<double>(nullable: true),
                    FLILOC = table.Column<double>(nullable: true),
                    FLIPRO = table.Column<double>(nullable: true),
                    FLIPAI = table.Column<double>(nullable: true),
                    FLIECI = table.Column<double>(nullable: true),
                    FLISEX = table.Column<double>(nullable: true),
                    FLIDEP = table.Column<double>(nullable: true),
                    FLILCM = table.Column<double>(nullable: true),
                    FLIDIS = table.Column<double>(nullable: true),
                    FLICOM = table.Column<double>(nullable: true),
                    FLITEF = table.Column<string>(maxLength: 255, nullable: true),
                    FLICUI = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Inscriptos",
                columns: table => new
                {
                    ins_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ins_ficha = table.Column<int>(nullable: true),
                    ins_tipflia = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ins_fecins = table.Column<string>(unicode: false, maxLength: 12, nullable: true),
                    ins_nombre = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ins_tipdoc = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    ins_numdoc = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ins_email = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    ins_telef = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    ins_estado = table.Column<string>(unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    ins_fecalt = table.Column<DateTime>(type: "smalldatetime", nullable: true, defaultValueSql: "(getdate())"),
                    IdUsuario = table.Column<int>(nullable: true),
                    IdDomicilio = table.Column<int>(nullable: true),
                    IdTipoFamilia = table.Column<int>(nullable: true),
                    CodigoVerificador = table.Column<Guid>(nullable: true, defaultValueSql: "(newid())"),
                    cuit_cuil = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    cuit_cuil_uno = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    cuit_cuil_dos = table.Column<string>(fixedLength: true, maxLength: 10, nullable: true),
                    ins_discapacitado = table.Column<int>(nullable: true),
                    ins_minero = table.Column<int>(nullable: true),
                    ins_veterano = table.Column<int>(nullable: true),
                    ins_fecUltAct = table.Column<DateTime>(type: "smalldatetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscriptos", x => x.ins_id);
                });

            migrationBuilder.CreateTable(
                name: "InsDomici",
                columns: table => new
                {
                    insd_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    insd_ficha = table.Column<int>(nullable: false),
                    insd_direcc = table.Column<string>(unicode: false, maxLength: 250, nullable: false),
                    insd_barrio = table.Column<string>(unicode: false, maxLength: 30, nullable: true),
                    insd_depar = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    insd_local = table.Column<string>(unicode: false, maxLength: 30, nullable: false),
                    insd_refer = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    insd_estado = table.Column<string>(unicode: false, fixedLength: true, maxLength: 1, nullable: false, defaultValueSql: "('A')"),
                    insd_fecalt = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "(getdate())"),
                    IdDepartamento = table.Column<int>(nullable: true),
                    IdLocalidad = table.Column<int>(nullable: true),
                    ins_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsDomici", x => x.insd_id);
                });

            migrationBuilder.CreateTable(
                name: "InsFamilia",
                columns: table => new
                {
                    insf_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    insf_ficha = table.Column<int>(nullable: true),
                    insf_tipflia = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    insf_nombre = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    insf_tipdoc = table.Column<string>(unicode: false, maxLength: 5, nullable: true),
                    insf_numdoc = table.Column<int>(nullable: false),
                    insf_estado = table.Column<string>(unicode: false, fixedLength: true, maxLength: 1, nullable: false, defaultValueSql: "('A')"),
                    insf_fecalt = table.Column<DateTime>(type: "smalldatetime", nullable: false, defaultValueSql: "(getdate())"),
                    ins_id = table.Column<int>(nullable: false),
                    ParentescoKey = table.Column<int>(nullable: true),
                    insf_discapacitado = table.Column<int>(nullable: true),
                    insf_minero = table.Column<int>(nullable: true),
                    insf_veterano = table.Column<int>(nullable: true),
                    FechaNacimiento = table.Column<string>(unicode: false, fixedLength: true, maxLength: 50, nullable: true),
                    fecNacDia = table.Column<string>(unicode: false, fixedLength: true, maxLength: 4, nullable: true),
                    fecNacMes = table.Column<string>(unicode: false, fixedLength: true, maxLength: 4, nullable: true),
                    fecNacAnio = table.Column<string>(unicode: false, fixedLength: true, maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsFamilia", x => x.insf_id);
                });

            migrationBuilder.CreateTable(
                name: "Parentesco",
                columns: table => new
                {
                    ParentescoKey = table.Column<int>(nullable: false),
                    ParentescoDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Parentes__65BFC8FA51A06095", x => x.ParentescoKey);
                });

            migrationBuilder.CreateTable(
                name: "SituacionLaboral",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(unicode: false, maxLength: 200, nullable: true),
                    IngresoNeto = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    TipoRevistaKey = table.Column<int>(nullable: true),
                    InscriptoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacionLaboral", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoEmpleo",
                columns: table => new
                {
                    TipoEmpleoKey = table.Column<int>(nullable: false),
                    TipoEmpleoDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoEmpl__B0AA76A0CBDB521B", x => x.TipoEmpleoKey);
                });

            migrationBuilder.CreateTable(
                name: "TipoFamilia",
                columns: table => new
                {
                    TipoFamiliaKey = table.Column<int>(nullable: false),
                    TipoFamiliaDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoFami__BDC7E2A4BD7E941C", x => x.TipoFamiliaKey);
                });

            migrationBuilder.CreateTable(
                name: "TipoIngreso",
                columns: table => new
                {
                    TipoIngresoKey = table.Column<string>(unicode: false, fixedLength: true, maxLength: 20, nullable: false),
                    TipoIngresoDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoIngr__428196550EF67648", x => x.TipoIngresoKey);
                });

            migrationBuilder.CreateTable(
                name: "TipoRevista",
                columns: table => new
                {
                    TipoRevistaKey = table.Column<int>(nullable: false),
                    TipoRevistaDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TipoRevi__5C615BB5772D7F90", x => x.TipoRevistaKey);
                });

            migrationBuilder.CreateTable(
                name: "TitularAS",
                columns: table => new
                {
                    FLIFIC = table.Column<double>(nullable: true),
                    FLISFI = table.Column<double>(nullable: true),
                    FLICFI = table.Column<double>(nullable: true),
                    FLIFIN = table.Column<double>(nullable: true),
                    FLIAPN = table.Column<string>(maxLength: 255, nullable: true),
                    FLITDO = table.Column<double>(nullable: true),
                    FLINDO = table.Column<int>(nullable: true),
                    FLINAC = table.Column<double>(nullable: true),
                    FLIFNA = table.Column<double>(nullable: true),
                    FLILOC = table.Column<double>(nullable: true),
                    FLIPRO = table.Column<double>(nullable: true),
                    FLIPAI = table.Column<double>(nullable: true),
                    FLIECI = table.Column<double>(nullable: true),
                    FLISEX = table.Column<double>(nullable: true),
                    FLIDEP = table.Column<double>(nullable: true),
                    FLILCM = table.Column<double>(nullable: true),
                    FLIDIS = table.Column<double>(nullable: true),
                    FLICOM = table.Column<double>(nullable: true),
                    FLITEF = table.Column<string>(maxLength: 255, nullable: true),
                    FLICUI = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    ClaveUsuario = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    usu_fecalt = table.Column<DateTime>(type: "smalldatetime", nullable: true, defaultValueSql: "(getdate())"),
                    usu_estado = table.Column<string>(unicode: false, fixedLength: true, maxLength: 1, nullable: true, defaultValueSql: "('A')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    LocalidadKey = table.Column<int>(nullable: false),
                    DepartamentoKey = table.Column<int>(nullable: false),
                    LocalidadDesc = table.Column<string>(unicode: false, fixedLength: true, maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Localida__710E3397E11306A9", x => new { x.LocalidadKey, x.DepartamentoKey });
                    table.ForeignKey(
                        name: "ILOCALIDAD1",
                        column: x => x.DepartamentoKey,
                        principalTable: "Departamento",
                        principalColumn: "DepartamentoKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_ins_numdoc",
                table: "Inscriptos",
                column: "ins_numdoc");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_DepartamentoKey",
                table: "Localidad",
                column: "DepartamentoKey");

            migrationBuilder.CreateIndex(
                name: "CIX_Usuario",
                table: "Usuario",
                column: "Id",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barrios");

            migrationBuilder.DropTable(
                name: "DirDupli");

            migrationBuilder.DropTable(
                name: "Direccion");

            migrationBuilder.DropTable(
                name: "Familia1");

            migrationBuilder.DropTable(
                name: "Familia2");

            migrationBuilder.DropTable(
                name: "Inscriptos");

            migrationBuilder.DropTable(
                name: "InsDomici");

            migrationBuilder.DropTable(
                name: "InsFamilia");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Parentesco");

            migrationBuilder.DropTable(
                name: "SituacionLaboral");

            migrationBuilder.DropTable(
                name: "TipoEmpleo");

            migrationBuilder.DropTable(
                name: "TipoFamilia");

            migrationBuilder.DropTable(
                name: "TipoIngreso");

            migrationBuilder.DropTable(
                name: "TipoRevista");

            migrationBuilder.DropTable(
                name: "TitularAS");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Departamento");
        }
    }
}
