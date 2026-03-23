### Configuration

Vous pouvez configurer la solution à l'aide du fichier `appsettings.shared.json` *(ou `appsettings.shared.Developement.json` pour la version de développement)* :

```json
{
  "AllowedHosts": "*",

  "Root": {
    "MailAddress" : <Adresse électronique de l'administrateur racine>,
    "Password"    : <Mot de passe de l'administrateur racine>
  },
  
  "DB" : {
    "Host"          : <Adresse de la base de données PostgreSQL>,
    "Port"          : <Port de la base de données PostgreSQL>,
    "Database"      : <Nom de la base de données PostgreSQL>,
    "Username"      : <Utilisateur de la base de données PostgreSQL>,
    "Password"      : <Mot de passe de la base de données PostgreSQL>,
    "EncryptionKey" : <Clé de chiffrement 128bit pour la base de données>
  },

  "Jwt" : {
    "Key" : {
      "Admin" : <Clé de chiffrement 512bit pour l'authentification administrateur>,
      "User"  : <Clé de chiffrement 512bit pour l'authentification utilisateur>
    },
    "Issuer"   : "cesizen.fr",
    "Audience" : "cesizen.fr",
    "Expiry"   : {
      "Admin" : <Durée de l'authentification administrateur (hh:mm)>,
      "User"  : <Durée de l'authentification utilisateur (hh:mm)>,
    }
  },

  "Pin" : {
    "RegistrationValidationRequestExpiry" : <Durée des codes PIN de création de compte utilisateur (hh:mm)>,
    "PasswordResetRequestExpiry"          : <Durée des codes PIN de réinistialisation de mot de passe utilisateur (hh:mm)>
  },

  "Smtp" : {
    "Host"        : <Adresse du service d'envoi de courrier électronique>,
    "Port"        : <Port du service d'envoi de courrier électronique>,
    "SenderEmail" : "noreply@cesizen.fr"
  },
  
  "Logging" : {
    "LogLevel" : {
      "Default" : "Information",
      "Microsoft.EntityFrameworkCore" : "Warning"
    }
  }
}
```

Il est possible de configurer la solution à l'aide de variables d'environnement : dans ce cas, nommez vos variables avec le format ``<Nom du parent>__<Nom de l'enfant>__<...>``. (exemple : ``Root__MailAddress=root@cesizen.fr``).

### Initialisation

Si aucune base de données et service d'envoi de courrier électronique ne sont en place, vous pouvez les mettre en place à l'aide de ce docker-compose :

```yml
services:

  db:
    image: postgres
    restart: always
    shm_size: 128mb
    ports:
      - <DB__Port>:5432
    environment:
      POSTGRES_PASSWORD: <DB__Password>
      POSTGRES_USER: <DB__Username>
      POSTGRES_DB: <DB__Database>

  smtp:
    image: maildev/maildev
    environment:
      MAILDEV_SMTP_PORT: 1026
    ports:
      - "3000:1080"         # Interface web
      - "<Smtp__Port>:1026" # SMTP
```

Clonez d'abord le dépôt en entrant cette commande dans le terminale :

```shell
git clone https://github.com/CDA-2025-CESI-Zen/CESI-Zen.git
```

### Exécution

Lancez l'**API Front-Office** *(nécessaire pour le fonctionnement du Front-Office mobile)* en entrant cette commande dans le terminale :

```shell
dotnet run --launch-profile https --project src\\Presentation\\FrontOffice.Api
```

- Pour initialiser la base de données lors de la première exécution, ajoutez `-- -n`
- Pour initialiser une base de données de développement, ajoutez `-- -n -d`

(exemple : ``dotnet run --launch-profile https --project src\\Presentation\\FrontOffice.Api -- -n -d``).

Lancez le **Back-Office** en entrant cette commande dans le terminale :

```shell
dotnet run --launch-profile https --project src\\Presentation\\BackOffice
```
