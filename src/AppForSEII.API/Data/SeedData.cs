namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            try {
                SeedTiposDeporte(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the TiposDeporte in the Database.");
            }
 
            try {
                SeedTiposMaterial(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the TiposMaterial in the Database.");
            }
 
            try {
                SeedMateriales(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Materiales in the Database.");
            }

                        try {
                SeedCompeticiones(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Competiciones in the Database.");
            }
            try
            {
                SeedClasesDeportivas(dbContext);
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "An error occurred seeding the ClasesDeportivas in the Database.");
            }
                    
 

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es", "12345678A", 30, "F");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es", "12345678B", 35, "M");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }


        public static void SeedTiposDeporte(ApplicationDbContext dbContext) {
            //it checks there is no TipoDeporte yet in the database
            if (!dbContext.TiposDeporte.Any()) {
                dbContext.TiposDeporte.AddRange(
                    // El "0" es intencional: es una columna de identidad (autoincremental),
                    // así que dejamos que la base de datos asigne el id real.
                    new TipoDeporte(0, "Fútbol", "Liga Local", 2),
                    new TipoDeporte(0, "Baloncesto", "Liga Regional", 1),
                    new TipoDeporte(0, "Tenis", null, 3)
                );
                dbContext.SaveChanges();
            }
        }
 
        public static void SeedTiposMaterial(ApplicationDbContext dbContext) {
            //it checks there is no TipoMaterial yet in the database
            if (!dbContext.TiposMaterial.Any()) {
                dbContext.TiposMaterial.AddRange(
                    new TipoMaterial(0, "Balón", new List<Material>()),
                    new TipoMaterial(0, "Raqueta", new List<Material>()),
                    new TipoMaterial(0, "Protección", new List<Material>())
                );
                dbContext.SaveChanges();
            }
        }
 
        public static void SeedMateriales(ApplicationDbContext dbContext) {
            //it checks there is no Material yet in the database
            if (!dbContext.Materiales.Any()) {
                var futbol = dbContext.TiposDeporte.First(t => t.Nombre == "Fútbol");
                var baloncesto = dbContext.TiposDeporte.First(t => t.Nombre == "Baloncesto");
                var tenis = dbContext.TiposDeporte.First(t => t.Nombre == "Tenis");
 
                var balon = dbContext.TiposMaterial.First(t => t.NombreTipoMaterial == "Balón");
                var raqueta = dbContext.TiposMaterial.First(t => t.NombreTipoMaterial == "Raqueta");
 
                // Orden del constructor: idMaterial, nombreMaterial, precio, cantidad, tipoMaterial, tipoDeporte, materialesAlquilados
                dbContext.Materiales.AddRange(
                    new Material(0, "Balón de fútbol reglamentario", 3.50m, 10, balon, futbol, new List<MaterialAlquilado>()),
                    new Material(0, "Balón de baloncesto", 2.50m, 8, balon, baloncesto, new List<MaterialAlquilado>()),
                    new Material(0, "Raqueta de tenis", 4.00m, 5, raqueta, tenis, new List<MaterialAlquilado>())
                );
                dbContext.SaveChanges();
            }}
        public static void SeedCompeticiones(ApplicationDbContext dbContext) {
            
            if (!dbContext.Competiciones.Any()) {
                var futbol = dbContext.TiposDeporte.First(t => t.Nombre == "Fútbol");
                var baloncesto = dbContext.TiposDeporte.First(t => t.Nombre == "Baloncesto");
                var tenis = dbContext.TiposDeporte.First(t => t.Nombre == "Tenis");

                
                dbContext.Competiciones.AddRange(
                    new Competicion("Liga Local de Futbol Sala", futbol, new DateTime(2026, 11, 15), "Pabellon Municipal", 20, 15.00m),
                    new Competicion("Torneo 3x3 de Baloncesto", baloncesto, new DateTime(2026, 11, 22), "Pista Exterior Norte", 12, 10.10m),
                    new Competicion("Open de Tenis de Otono", tenis, new DateTime(2026, 12, 5), "Pistas de Tenis", 16, 20.50m),
                    new Competicion("Torneo Benefico de Futbol 7", futbol, new DateTime(2026, 12, 13), "Campo Anexo", 0, 12.00m)
                );
                dbContext.SaveChanges();
            }
        }
        }

        public static void SeedClasesDeportivas(ApplicationDbContext dbContext){
            
            if (!dbContext.ClasesDeportivas.Any())
            {
                var futbol = dbContext.TiposDeporte.First(t => t.Nombre == "Fútbol");
                var baloncesto = dbContext.TiposDeporte.First(t => t.Nombre == "Baloncesto");
                var tenis = dbContext.TiposDeporte.First(t => t.Nombre == "Tenis");

                dbContext.ClasesDeportivas.AddRange(

                    new ClaseDeportiva(
                        0,
                        "Entrenamiento de técnica y posesión",
                        DateTime.Today.AddDays(1).AddHours(18),
                        "Pista 1",
                        "Carlos Ruiz",
                        "Intermedio",
                        20,
                        7.50m,
                        futbol,
                        futbol.Id,
                        new List<ClaseInscrita>()
                    ),

                    new ClaseDeportiva(
                        0,
                        "Iniciación al baloncesto",
                        DateTime.Today.AddDays(2).AddHours(19),
                        "Pista 2",
                        "Marta López",
                        "Iniciación",
                        30,
                        6.00m,
                        baloncesto,
                        baloncesto.Id,
                        new List<ClaseInscrita>()
                    ),

                    new ClaseDeportiva(
                        0,
                        "Perfeccionamiento de saque y volea",
                        DateTime.Today.AddDays(3).AddHours(17).AddMinutes(30),
                        null,
                        "Ana Martín",
                        "Avanzado",
                        8,
                        10.00m,
                        tenis,
                        tenis.Id,
                        new List<ClaseInscrita>()
                    )
                );

                dbContext.SaveChanges();
            }
        }


    }
