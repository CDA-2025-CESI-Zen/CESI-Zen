using System.Globalization;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Diagnoses;
using FluentResponse;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Presentation.FrontOffice.Api.Core;
public static partial class Extensions {

    private static readonly Dictionary<string, int> DefaultDiagnosisItems = new() {
        {"Mort du conjoint", 100},
        {"Divorce", 73},
        {"Séparation", 65},
        {"Maladie grave ou blessure personnelle", 53},
        {"Mariage", 50},
        {"Licenciement", 47},
        {"Réconciliation conjugale", 45},
        {"Retraite", 45},
        {"Changements majeurs dans la santé d'un membre de la famille", 44},
        {"Grossesse", 40},
        {"Difficultés sexuelles", 39},
        {"Ajout d'un nouveau membre de la famille", 39},
        {"Changement majeur dans les conditions de travail", 39},
        {"Changement majeur dans les finances", 38},
        {"Mort d'un proche", 37},
        {"Changement de responsabilités au travail", 29},
        {"Départ d'un enfant du foyer", 29},
        {"Problèmes avec les beaux-parents", 29},
        {"Succès exceptionnel au travail", 28},
        {"Début ou fin des études", 26},
        {"Changement dans les conditions de vie", 25},
        {"Révision de ses habitudes personnelles", 24},
        {"Problèmes avec son supérieur", 23},
        {"Changement dans les horaires de travail", 20},
        {"Changement de résidence", 20},
        {"Changement d'école", 20},
        {"Changement dans les activités de loisirs", 19},
        {"Changement dans les activités sociales", 19},
        {"Changement majeur dans les habitudes de sommeil", 16},
        {"Changement dans les habitudes alimentaires", 15},
        {"Vacances", 13},
        {"Noël", 12},
        {"Infractions mineures à la loi", 11}
    };

    private static readonly Dictionary<int, string> DefaultDiagnosisAnalyses = new() {
        {0, "Une incidence relativement faible sur la vie et une faible sensibilité aux problèmes de santé provoqués par le stress."},
        {150, "Une probabilité d'environ 50 % de connaître de sérieux problèmes de santé dans les deux prochaines années."},
        {330, "Plus de 80 % de chances de connaître de sérieux problèmes de santé dans les deux prochaines années, selon le modèle statistique de prédiction Holmes-Rahe."},
    };

    private static readonly string[] DefaultCategoryNames = [
        "Communication",
        "Cultures",
        "Développement personnel",
        "Intelligence émotionnelle",
        "Loisirs",
        "Monde professionnel",
        "Parentalité",
        "Qualité de vie",
        "Recherche de sens",
        "Santé physique",
        "Santé psychique",
        "Spiritualité",
        "Vie affective",
    ];

    private const string LoremIpsum = """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam eget condimentum sem, vitae hendrerit erat. Pellentesque bibendum enim libero, vehicula porttitor nibh convallis at. Quisque vehicula sem vel est elementum, vel mattis dui fringilla. Etiam finibus quis sem vitae ultrices. Vivamus blandit tortor nec mi posuere tristique. Curabitur vel neque sit amet sem suscipit ornare vel consectetur est. Aliquam id risus ac felis iaculis aliquam a sed magna. Aliquam enim purus, condimentum sit amet dignissim ut, consectetur vel est. Duis ac metus sem. In vulputate eros ut dolor consectetur, eget rutrum ante malesuada.
        Morbi a mi semper, malesuada diam in, dapibus eros. Vivamus tincidunt lobortis leo, eu scelerisque neque tristique a. Nullam mauris arcu, lacinia eget augue ac, volutpat imperdiet augue. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Proin bibendum, libero id scelerisque finibus, felis augue tristique est, et tristique eros nibh vel urna. Ut ac est magna. Nulla luctus libero ac magna finibus aliquam. In hac habitasse platea dictumst. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean ac accumsan orci. Pellentesque finibus sit amet nunc id pretium. Sed a ligula mollis, molestie lacus non, fringilla leo.
        Nullam auctor, velit nec feugiat dapibus, lacus ipsum tempus ante, eget dignissim mi ipsum ac mauris. Etiam porta euismod massa, quis porttitor nunc vehicula non. Proin sollicitudin libero sapien, non sodales sapien eleifend eu. Morbi quis urna sit amet felis cursus vehicula quis non nulla. Sed vel ligula mi. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Curabitur fermentum ultrices lorem, quis euismod felis mattis a.
        Pellentesque sollicitudin felis nec vestibulum facilisis. Mauris eget elit in dolor ultricies tempus. Ut metus mauris, tristique in tristique quis, tempus a sem. Sed euismod lorem lacus, ut auctor erat aliquam nec. Suspendisse quam dui, congue ut commodo ac, posuere in erat. Pellentesque eu ante sed nulla pulvinar auctor vitae in elit. Cras malesuada cursus enim vitae bibendum. Quisque volutpat accumsan risus a porttitor. Integer ullamcorper, lectus vitae scelerisque auctor, tortor urna tincidunt dui, quis malesuada lorem ex nec sem. Quisque et mi ac ipsum consectetur egestas.
        Aliquam erat volutpat. Fusce metus erat, euismod nec urna ut, iaculis fermentum mauris. Ut ac tincidunt nisi. In scelerisque dui ante, id sodales dolor tempor ac. Morbi elementum vel tellus vel rutrum. Integer porta ornare lectus non egestas. Maecenas pellentesque erat ex, a consequat metus pretium sit amet. Suspendisse tellus orci, convallis in mollis ac, egestas in nisl. Donec aliquet dignissim feugiat. Nullam euismod nec ex ut tempus. Mauris sit amet bibendum mi, at aliquet sem. Donec et laoreet nisl. Nullam pharetra fermentum enim, a pretium lacus pellentesque id. Mauris facilisis hendrerit odio, ut aliquam ex blandit eget. Vivamus facilisis mattis pharetra.
        """;

    /// <summary>
    /// Initializes the database.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitDb(this WebApplication app, bool forceInit, bool dev) {

        using (var scope = app.Services.CreateScope()) {
            app.Logger.LogInformation("Database initialization started...");

            using var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

            if (forceInit) dbContext.Database.EnsureDeleted();
            if (dbContext.Database.EnsureCreated() || forceInit) {
            
                dbContext.Add(Admin.TryCreate(app.Configuration["Root:MailAddress"]!, app.Configuration["Root:Password"]!).Unwrap());
                dbContext.SaveChanges();
                dbContext.InitDiagnosisItems();
                dbContext.InitDiagnosisAnalyses();
                dbContext.InitCategories();

                if (dev) {
                    dbContext.InitUsers();
                    dbContext.InitPages();
                }
            }
        }

        app.Logger.LogInformation("Database initialization completed!");

    }

    private static void InitUsers(this DbContext dbContext) {

        Random random = new();
        
        string GeneratePassword() {

            int    randValue;
            string password = "";
            char   character;

            for (int i = 0; i < 4; i++) {

                randValue = random.Next(0, 26);
                character = Convert.ToChar(randValue + 'a');
                password += character;
                
            } for (int i = 0; i < 4; i++) {

                randValue = random.Next(0, 26);
                character = Convert.ToChar(randValue + 'A');
                password += character;

            } for (int i = 0; i < 4; i++) {

                randValue = random.Next(0, 10);
                character = Convert.ToChar(randValue + '0');
                password += character;

            }

            return password;
        }

        dbContext.AddRange(
            Enumerable.Range(0, 100).Select(i =>
                User.TryCreate(
                    mailAdress : $"user{i}@domaine.com",
                    password   : GeneratePassword()
                ).OnSuccess(x => ((random.Next() & 1) == 1) ? x.AsAnonymized() : x).Unwrap()
            )
        );

        dbContext.SaveChanges();
    }

    private static void InitDiagnosisItems(this DbContext dbContext) {
        dbContext.AddRange(DefaultDiagnosisItems.Select(x => DiagnosisItem.TryCreate(x.Key, x.Value).Unwrap()));
        dbContext.SaveChanges();
    }

    private static void InitDiagnosisAnalyses(this DbContext dbContext) {
        dbContext.AddRange(DefaultDiagnosisAnalyses.Select(x => DiagnosisAnalysis.Create(x.Key, x.Value)));
        dbContext.SaveChanges();
    }

    private static void InitCategories(this DbContext dbContext) {
        dbContext.AddRange(DefaultCategoryNames.Select(name => Category.TryCreate(name).Unwrap()));
        dbContext.SaveChanges();
    }

    private static void InitPages(this DbContext dbContext) {

        var random = new Random();
        var categories = dbContext.Set<Category>().ToList();

        dbContext.AddRange(
            Enumerable.Range(0, 50).Select(i =>
                Page.TryCreate(
                    title    : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LoremIpsum[i..(i+random.Next(5, 64))].Trim(',', '.', ' ').ToLower()),
                    category : categories[random.Next(categories.Count)],
                    content  : LoremIpsum[0..random.Next(256, LoremIpsum.Length)].Trim(',', '.', ' ')
                ).Unwrap()
            )
        );
        dbContext.SaveChanges();
    }
}