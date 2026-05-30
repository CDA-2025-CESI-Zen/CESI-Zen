### Initialiser le projet

Clonez d'abord le dépôt en entrant cette commande dans le terminale :

```shell
git clone https://github.com/CDA-2025-CESI-Zen/CESI-Zen.git
```

### Configurer la solution

Vous pouvez configurer la solution à l'aide du fichier `.env` (un exemple est donné avec `.env.example`) :

```env
Root__MailAddress = <Adresse électronique de l'administrateur racine>
Root__Password    = <Mot de passe de l'administrateur racine>

DB__Host          = <Adresse de la base de données PostgreSQL>
DB__Port          = <Port de la base de données PostgreSQL>
DB__Database      = <Nom de la base de données PostgreSQL>
DB__Username      = <Utilisateur de la base de données PostgreSQL>
DB__Password      = <Mot de passe de la base de données PostgreSQL>
DB__EncryptionKey = <Clé de chiffrement 128bit pour la base de données>

Jwt__Key__Admin      = <Clé de chiffrement 512bit pour l'authentification administrateur>
Jwt__Key__User       = <Clé de chiffrement 512bit pour l'authentification utilisateur>
Jwt__Issuer          = "resr.fr"
Jwt__Audience        = "resr.fr"
Jwt__Expiry__Admin = <Durée de l'authentification administrateur (hh:mm)>
Jwt__Expiry__User  = <Durée de l'authentification utilisateur (hh:mm)>

Pin__RegistrationValidationRequestExpiry = <Durée des codes PIN de création de compte utilisateur (hh:mm)>
Pin__PasswordResetRequestExpiry          = <Durée des codes PIN de réinistialisation de mot de passe utilisateur (hh:mm)>

Smtp__Host        = <Adresse du service d'envoi de courrier électronique>
Smtp__Port        = <Port du service d'envoi de courrier électronique>
Smtp__SenderEmail = "noreply@cesizen.fr"
  
Logging__LogLevel__Default                       = "Information"
Logging__LogLevel__Microsoft.EntityFrameworkCore = "Warning"
```

### Hébergement Docker
#### Configuration

Configurez votre `docker-compose.yml` (un exemple est donné avec `docker-compose.example.yml` et `docker-compose.dev.example.yml`).
Puis, assurez vous que le dossier `%USERPROFILE%/.aspnet/https/` existe avant d'entrez cette commande dans le terminal powershell en remplaçant `ASPNETCORE_Kestrel__Certificates__Default__Password` par la valeur configurée dans le `.env` :

```shell
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p <ASPNETCORE_Kestrel__Certificates__Default__Password> --trust
```

\* Si vous êtes sur MacOS, remplacez la variable d'environnement `USERPROFILE` par `HOME`.

##### Exécution

Si vous souhaitez démarrer la solution en environnement de developpement, entrez cette commande :

```shell
docker compose -f docker-compose.dev.yml up -d --build
```

Si vous souhaitez démarrer la solution en environnement de production, entrez cette commande :

```shell
docker compose -f docker-compose.yml up -d --build
```

L'interface administrateur sera accessible sur `http://localhost` et automatiquement redirigée vers `https://localhost`.
L'API sera accessible sur `http://localhost:8080` et automatiquement redirigée vers `https://localhost:8443`.


### Hébergement local
Si vous souhaitez démarrer la solution localement, entrez cette commande après avoir initialisé les variables d'environnement :

```shell
dotnet run --launch-profile https --project src\\Presentation\\BackOffice
```

Puis, entrez :

```shell
dotnet run --launch-profile https --project src\\Presentation\\FrontOffice.Api --
```

\* Si vous souhaitez initialiser la base de données, ajoutez `-i` ou `--init-db`.
\* Si vous souhaitez indiquer la base de données avec des données de test, ajoutez aussi `-d` ou `--dev`.
\* Si vous souhaitez forcer cette action en réinitialisant la base de données, ajoutez aussi `-f` ou `--force-init`.