# SQS-Vorlesung

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=laternenpolster_sqs-vorlesung&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=laternenpolster_sqs-vorlesung)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=laternenpolster_sqs-vorlesung&metric=coverage)](https://sonarcloud.io/summary/new_code?id=laternenpolster_sqs-vorlesung)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=laternenpolster_sqs-vorlesung&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=laternenpolster_sqs-vorlesung)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=laternenpolster_sqs-vorlesung&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=laternenpolster_sqs-vorlesung)

**Über arc42**

arc42, das Template zur Dokumentation von Software- und
Systemarchitekturen.

Template Version 8.2 DE. (basiert auf AsciiDoc Version), Januar 2023

Created, maintained and © by Dr. Peter Hruschka, Dr. Gernot Starke and
contributors. Siehe <https://arc42.org>.

# Einführung und Ziele

Bei der hier beschriebenen Software handelt es sich um eine ASP.NET Web-App, mit der Informationen zu beliebigen Pokémons abgerufen werden können. Die App wird von einer Gruppe an Pokémon-Spielern eingesetzt, diese besteht aus 30 Leuten. Die Spieler erwarten sich durch die App, ergänzend zum Spiel Informationen zu Pokémons einsehen zu können.

Die Informationen werden von einer externen Pokédex REST-API abgerufen und in einer PostgreSQL Datenbank gespeichert, wodurch die Anfragen auf die API reduziert werden sollen. Diese Komponenten werden in der Zukunft möglicherweise durch andere Anbieter ausgetauscht. Aus diesem Grund soll die Architektur der App dafür ausgelegt sein, dass einzelne Module möglichst leicht ausgetauscht werden können. Als Grundlage dient hierfür die "Clean Architecture" von Jason Taylor.

Das Ziel der Software ist darüber hinaus nicht, eine komplexe neue App zu entwickeln, sondern die Software-Qualität möglichst gut abzusichern.

## Aufgabenstellung

| Use-Case                | Beschreibung                                                                                                                                                                                                                                                                    | Stakeholder |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------- |
| 1. Abrufen über Website | Informationen zu einem bestimmten Pokémon sollen über die Website abgerufen werden. Dabei sucht der Spieler das Pokémon anhand seines Namens, den er auf der Startseite der Web-App eingibt. Existiert kein Pokémon mit diesem Namen, soll eine Fehlermeldung angezeigt werden. | Spieler     |
| 2. Abrufen über API     | Informationen zu Pokémons können darüber hinaus auch durch eine integrierte REST-API abgerufen werden. Diese sucht ein Pokémon anhand seines Namens und liefert das Ergebnis als JSON kodiert zurück.                                                                           | Spieler     |

## Qualitätsziele

| Prio | Qualitätskriterium  | Ziele                                                                                                                                                                                                                                    | Maßnahmen                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| ---- | ------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 1    | Wartbarkeit         | - Einfache Weiterentwicklung des Projekts<br>- Hohe Verständlichkeit und Qualität des Codes<br>- Komponenten wie Datenbank oder API müssen zentral ausgetauscht werden können.<br>- Nur 1 Programmiersprache für alle Tests, keine DSLs. | - Projektstruktur mit 9 Projekten nach [Clean Architecture](https://jasontaylor.dev/clean-architecture-getting-started/) von Jason Taylor.<br>- Zyklische Abhängigkeiten werden bereits durch den Compiler erkannt.<br>- Alle Tests werden durch C# Projekte umgesetzt. Entwickler müssen somit keine DSLs für verschiedene Tests beherrschen.<br>- Code Style und Conventions werden durch die Pipeline geprüft. Bereits bei einem einzigen Verstoß oder Warnung schlägt die Pipeline fehl.<br>- Architektur Regeln werden durch ArchUnit Tests geprüft.<br>- Code Qualität wird mit SonarCloud überprüft.<br>- Gesamter Code (inklusive Tests) ist mit Kommentaren versehen.<br>- Dependency Injection in Kombination mit Interfaces für erhöhte Modularität. |
| 2    | Funktionale Eignung | - Die App muss die Bedürfnisse / Anforderungen der Benutzer erfüllen.                                                                                                                                                                    | - Projekt wird auf mehreren Ebenen funktional getestet: Unit Tests, Integration Tests, E2E Tests. Letztere testen speziell zuvor definierte Use-Cases.<br>- Erreichte Coverage: 100%                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| 3    | Reliability         | - Die App soll auch bei deutlich höherer Last (500 statt 30 parallelen Anfragen) jede Anfrage ohne Fehler beantworten.                                                                                                                   | - Last Tests mit NBomber<br>- SonarCloud mit 0 Reliability Warnungen                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| 4    | Security            | - Der Code soll keine bekannten Sicherheitslücken aufweisen und neue automatisch erkennen.                                                                                                                                               | - Aqasecurity Trivy Vulnerability Scanner in GitHub Action<br>- CodeQL Scanner für C# und JS/TS<br>- SonarCloud mit 0 offenen Security Warnungen<br>- Regelmäßige Ausführung der Security-Pipelines, auch ohne Änderungen am Repo                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| 5    | Usability           | - Die Funktionalität der UI soll automatisiert sichergestellt werden.<br>- Pokémons im Cache müssen innerhalb von 30ms aufgerufen werden können.                                                                                         | - Frontend Tests mit Playwright (E2E Tests)<br>- Schnelle durchschnittliche Ladezeiten für Pokémons im Cache (< 30ms) durch Last Tests sichergestellt, auch bei 500 parallelen Anfragen.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |

## Stakeholder

| Rolle         | Kontakt    | Erwartungshaltung                                                             |
| ------------- | ---------- | ----------------------------------------------------------------------------- |
| Spieler       | -          | Zuverlässiges Abrufen von Pokémon Informationen                               |
| Entwickler    | -          | Gute Dokumentation, gute Projektstruktur, hohe Code Qualität                  |
| API-Betreiber | pokeapi.co | Keine Überlastung der API durch viele Anfragen                                |
| Dozent        | -          | Vollständige Dokumentation des Projekts, Verifizierung der Qualitätskriterien |

# Randbedingungen

Die folgenden Anforderungen wurden mündlich im Rahmen der Vorlesung "Software Qualitätssicherung" festgelegt:

| Name            | Beschreibung                                                         |
| --------------- | -------------------------------------------------------------------- |
| Frontend        | Die App muss sowohl ein Web-Frontend als auch eine REST-API anbieten |
| Kontext         | Eine Datenbank und eine externe REST-API müssen angebunden werden    |
| Tests           | Mindestens müssen Unit- und Integration Tests vorhanden sein         |
| GitHub          | Code und Doku müssen vollständig auf GitHub abrufbar sein            |
| Automatisierung | Tests müssen automatisiert mit GitHub Actions ausgeführt werden      |
| Deployment      | Das Projekt kann über Docker installiert werden                      |
| Code Qualität   | SonarCloud muss über GitHub Actions benutzt werden                   |

# Kontextabgrenzung

## Fachlicher Kontext

![](images/KontextFachlich.png)

Übersicht, welche Daten über welche Schnittstelle kommuniziert werden:

| Sender              | Empfänger      | Daten                                                                                                                                                                                                                                                                                                      |
| ------------------- | -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Pokémon Lookup      | Spieler        | Gefilterte Pokémon Eigenschaften. Im Rahmen dieser App sind nur die Informationen "Pokémon ID", "Name", "Weight" und "Height" relevant. Andere Daten werden nicht gespeichert.<br>Der Umfang und die Namen der Eigenschaften werden durch die App festgelegt und bleiben gleich, wenn sich die API ändert. |
| Externe Pokédex API | Pokémon Lookup | Alle in der Pokédex API verfügbaren Pokémon Informationen.<br>Der Umfang und die Namen der Eigenschaften werden durch die API vorgegeben, die App hat keinen Einfluss darauf. Um die Daten in der App nutzen zu können, müssen sie erst in "gefilterte Daten" umgewandelt werden.                          |
| Pokémon Lookup      | Pokémon Cache  | gefilterte Pokémon Eigenschaften.                                                                                                                                                                                                                                                                          |

Die Website und API des Pokémon Lookup Systems können folgendermaßen aufgerufen werden:

| Art     | Aufruf                                                                                                                                 |
| ------- | -------------------------------------------------------------------------------------------------------------------------------------- |
| Website | 1. Navigation auf Startseite der App<br>2. Eingabe eines Suchbegriffs<br>3. Klick auf Suche<br>4. Details zum Pokémon werden angezeigt |
| Website | Pokemon Informationen können direkt unter `/Pokemon?name={Dein Suchbegriff}` abgerufen werden.                                         |
| API     | Pokemon Informationen können direkt unter `/api/v1/pokemon/{Dein Suchbegriff}` abgerufen werden.                                       |


Wie die jeweiligen Daten-Umfänge implementiert sind, wird im Folgenden beschrieben.

## Technischer Kontext

![](images/Kontext.png)

Der Inhalt des gestrichelten Kastens wird im Rahmen dieses Projekts entwickelt. Weitere Informationen zu den Schnittstellen:

|                                     | External Pokédex API                 | Pokémon Cache                               |
| ----------------------------------- | ------------------------------------ | ------------------------------------------- |
| Adresse                             | https://pokeapi.co                   | Lokal in Docker Compose. Hostname: postgres |
| Endpunkt                            | /api/v2/pokemon/{name}               | -                                           |
| Ausgetauschte Daten                 | PokedexResultDto.cs (Infrastructure) | Pokemon.cs (Domain)                         |
| Verantwortliche Software-Komponente | IPokemonApiRequester.cs              | ICachingService.cs                          |
| Verwendete Implementierung          | PokemonApiRequester.cs               | DatabaseCachingService.cs                   |

# Lösungsstrategie

- Als Programmiersprache wird C# in Kombination mit ASP.NET 8.0 verwendet. Diese ist dem Entwickler bereits bekannt und kann auch in allen Tests verwendet werden. Für alle Tests wird einheitlich xUnit verwendet, da es eine kompakte Schreibweise für Assertions verwendet.
- Die Software wird in 9 C# Projekte unterteilt, die durch 1 Solution verbunden sind. Die Trennung entspricht der Clean Architecture von Jason Taylor. Im Mittelpunkt stehen dabei die Projekte "Domain", "Application", "Infrastructure" und "Web". Hierdurch wird eine gute Wartbarkeit und Erweiterbarkeit ermöglicht, auch bei deutlich größeren Projekten. Getestet werden die Regeln durch Architektur Tests und den Compiler selbst.
- Um die Datenbank-Kommunikation robust zu machen und vor SQL-Injections zu schützen wird Entity-Framework verwendet. Selbst entwickelte SQL-Querys kommen nicht zum Einsatz.
- Um eine korrekte Funktion der App zu gewährleisten, wird mit 5 verschiedenen Test-Projekten eine Coverage von insgesamt 100% erreicht.

# Bausteinsicht

![](images/Bausteinsicht.png)

## Whitebox Gesamtsystem

Die Zerlegung des Gesamtsystems orientiert sich an der Struktur von Clean Architecture. Die Projekte Domain, Application, Infrastructure und Web stellen dabei zentrale Komponenten der Software dar. Die Namen und Funktionen der Komponenten sind durch Clean Architecture fest geregelt.

### Enthaltene Bausteine

| **Name**       | **Verantwortung**                                                                                                                               |
| -------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| Pokémon Lookup | Eine ASP.NET WebApp, die Suchanfragen der Benutzer entgegennimmt und mit Pokémon-Details beantwortet. Diese App wird in diesem Repo entwickelt. |

### Schnittstellen

- Player - Pokémon Lookup: HTTP / REST (JSON)
- Pokémon Lookup - Pokédex API: REST (JSON)
- Pokémon Lookup - Pokémon Cache: EF-Core

## Ebene 1
![](https://jasontaylor.dev/wp-content/uploads/2020/01/Figure-01-2.png)

Quelle: https://jasontaylor.dev/clean-architecture-getting-started/

### Whitebox Domain

- Enthält Klassen, die über die Grenzen der Applikation in einem Unternehmen Relevanz haben. Sie werden auch als "Enterprise Logic" beschrieben. Diese Schicht darf keine Abhängigkeiten haben (z.B. Datenbank, REST-Anfragen).
### Whitebox Application

- Business Logic, die im Rahmen der WebApp Verwendung findet. Hier dürfen ebenfalls keine Abhängigkeiten vorhanden sein.
### Whitebox Infrastructure

- In dieser Schicht werden u.a. Interfaces aus Application implementiert. Abhängigkeiten wie zu einer Datenbank sollen hier sein.
- Schnittstellen: Pokémon Cache, Pokédex API
### Whitebox Web

- Das Frontend der WebApp, auch als Presentation Layer bezeichnet. Hier wird keine neue Business Logic eigeführt, sondern die anderen Komponenten kombiniert. So wird ermöglicht, dass das Frontend mit möglichst wenig Aufwand gegen ein anderes getauscht wird. Abhängigkeiten zu Infrastructure dürfen jedoch nicht bestehen. (Einzige Ausnahme: Beim Setup der Dependency Injection in der Klasse Program.cs)
- Schnittstellen: HTTP und REST zum Spieler

## Ebene 2

### Whitebox Domain

- **Entities**: Enthält Entities, die unternehmensweit Bedeutung haben. Ein Beispiel hierfür ist die Datenklasse Pokemon.cs

### Whitebox Application

- **Services**: Enthält Interfaces für alle Komponenten der App. Implementiert werden diese erst durch Infrastructure.
- **Exceptions**: Exceptions, die über mehrere Schichten hinweg zur Kommunikation verwendet werden.

### Whitebox Infrastructure

- **Data**: Enthält die EF-Core Context-Klasse, die zur Kommunikation zur Datenbank verwendet wird. 
- **Caching**: Komponente, die Datenbankoperationen für den Pokémon Cache kapselt.
- **ExternalLookup**: Komponenten zur Kommunikation mit einer beliebigen Pokédex REST-API. Außerdem existiert hier eine Komponente, die das Zusammenspiel aus Cache und Pokédex API koordiniert.

### Whitebox Web
Die enthaltenen Komponenten werden nicht genauer erläutert, da es sich um Standards bei einer ASP.NET MVC WebApp handelt.

- **Views**: ASP.NET Razor Pages, die durch einen Controller erzeugt werden. Sie sind das eigentliche Frontend der WebApp.
- **Controllers**: Verarbeitung von Benutzer-Anfragen auf bestimmten Endpunkten. Hierdurch wird z.B. bei einer Anfrage auf die Startseite der WebApp das entsprechende Frontend (=Views) zurückgegeben.
- **Models**: ViewModels, die vom Controller an Views übergeben werden. Hierdurch werden die Daten bestimmt, die z.B. in Feldern im Frontend angezeigt werden.

# Laufzeitsicht

## Abrufen eines Pokémons ohne Existenz im Cache

Ein Spieler ruft ein Pokémon über die Website der App ab, das bis jetzt nicht im Cache gespeichert war. Es muss von der Pokédex API abgerufen werden und im Cache gespeichert werden.
![](images/AblaufNotCached.png)

ASP.NET Controller ist im Diagramm eine Zusammenfassung aus verschiedenen Komponenten der Software, um das Diagramm übersichtlicher zu machen. Die eigentliche Logik wird von mehreren Services ausgeführt, die die Business Logic trennen und eigene Fehler einführen. Diese werden in [Bausteinsicht](#Bausteinsicht) erläutert.

# Verteilungssicht

Die Software wird in Form von Docker-Compose bereitgestellt und läuft auf einem einzelnen System. Dadurch ist die verwendete Hard- und Software auf dem Zielsystem nebensächlich.
Außerdem wird die Zuverlässigkeit und Portabilität erhöht, da die Software isoliert läuft und durch Docker Compose die Verbindung zur Datenbank identisch bleibt.

Zum Start des Docker Compose Projekts müssen folgende Umgebungsvariablen konfiguriert sein:

| Name              | Wert                                                             |
| ----------------- |------------------------------------------------------------------|
| DATABASE_USER     | PostgreSQL Benutzername, verwendet für den Cache                 |
| DATABASE_PASSWORD | PostgreSQL Passwort                                              |
| DATABASE_DB       | Datenbank, die auf einem PostgreSQL Server verwendet werden soll |
| DATABASE_SERVER   | Hostname des PostgreSQL Servers                                  |
| DATABASE_PORT     | Port des PostgreSQL Servers                                      |

Anschließend kann das Projekt folgendermaßen gestartet werden:
```sh
cd PokemonLookup
docker compose up
```

![](images/Verteilungssicht.png)


# Querschnittliche Konzepte

## Clean Architecture
Die Entscheidung mit dem größten Einfluss auf die gesamte Software ist die Verwendung von Clean Architecture. Wie bereits unter [Bausteinsicht](#Bausteinsicht) beschrieben, wird der Code der Anwendung in die Projekte "Domain", "Application", "Infrastructure" und "Web" unterteilt. Inhalt und Funktion der Projekte wird ebenfalls unter [Bausteinsicht](#Bausteinsicht) beschrieben.

Dadurch ergeben sich laut Jason Taylor folgende Vorteile:
- Unabhängigkeit von Frameworks
- Einfache Testbarkeit
- Unabhängigkeit zum Frontend und Datenbank
- Domain und Application sind unabhängig zu jeglichem externen Code

Mehr Informationen zu Clean Architecture finden sich [hier](https://jasontaylor.dev/clean-architecture-getting-started/).

## Inversion of Control
Das Projekt "Web" verwendet Dependency Injection, um Inversion of Control zu ermöglichen. Besonders ist, dass Komponenten bereits in Projekten außerhalb von Web transitiv voneinander abhängen können.

Eine Komponente darf nur über eine Funktionalität verfügen (z.B. "Cache Lesen / Schreiben" oder "Externe API anfragen"). Komponenten müssen immer über ein Interface verfügen, über dass die Funktionalität der Komponente verwendet werden kann. Dieses wird in der Dependency Injection registriert und von andern Klassen oder Komponenten verwendet. Eine direkte Instanziierung oder Verwendung der Komponente ohne Interface darf nicht stattfinden, außer in Tests.

Von dieser Regelung nicht betroffen sind Controller, Views, ViewModels und Datenklassen.

## Code Stil / Qualität
Der Code Stil muss dem Standard bei der C# (Version 12.0) Entwicklung entsprechen. Diese Regel wird durch GitHub Actions überprüft. Alle Tests müssen in C# entwickelt sein, damit der Entwickler möglichst wenig Sprachen beherrschen muss.
Alle Komponenten oder Änderungen in diesen müssen mindestens durch Unit Tests getestet werden. Umfangreichere Änderungen erfordern eine Anpassung von Integration-, E2E-, Load- oder Architektur-Tests.

# Architekturentscheidungen

Die zentrale Architekturentscheidung ist Clean Architecture von Jason Taylor. Diese und weitere Entscheidungen werden bereits unter [Bausteinsicht](#Bausteinsicht) und [Querschnittliche Konzepte](#Querschnittliche-Konzepte) beschrieben und werden deshalb hier nicht wiederholt.

# Qualitätsanforderungen

## Qualitätsbaum
![](images/Qualitätsbaum.png)

## Qualitätsszenarien

| Kriterium              | Szenario                                                                                                                                                                      | Typ      | Maßnahme                                                   |
| ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------- | ---------------------------------------------------------- |
| Wartbarkeit            | Die Datenbank für den Cache soll auf MySQL geändert werden.                                                                                                                   | Änderung | Dependency Injection                                       |
|                        | Ein neuer Entwickler fügt eine Abhängigkeit zu einer externen Library im Domain Projekt ein.                                                                                  | Änderung | Architektur Tests                                          |
|                        | Ein neuer Entwickler möchte im Application Projekt auf eine Klasse aus Web zugreifen. Er fügt deshalb eine Referenz zu dem Projekt hinzu.                                     | Änderung | Zyklische Abhängigkeiten durch Compiler, Architektur Tests |
|                        | Die App ist ein großer Erfolg und soll zukünftig auch als Handy App angeboten werden.                                                                                         | Änderung | Clean Architecture                                         |
|                        | Ein neuer Entwickler soll Tests für die App entwickeln. Er kann jedoch nur C#.                                                                                                | Änderung | Einheitliche Sprache für Tests                             |
|                        | Das Projekt wird nach längerer Pause weiterentwickelt. Alle Komponenten müssen neu entwickelt werden, da der Code nicht verstanden wird.                                      | Änderung | Code + Tests vollständig dokumentiert                      |
|                        | Es arbeiten mehrere Entwickler am Projekt. Nach einigen Änderungen weist jede Klasse andere Coding Conventions auf. Die meisten entsprechen nicht dem Standard.               | Änderung | Code Style / Convention Test in Pipeline, SonarCloud       |
| Funktionale Eignung    | Eine wichtige Komponente der App wurde optimiert. Wurde jeder Edge-Case berücksichtigt?                                                                                       | Änderung | Unit-, Integration-Tests                                   |
|                        | Version 2.0.0 wird veröffentlicht. Entspricht die App noch allen Erwartungen?                                                                                                 | Änderung | Integration-, E2E-, Last-Test                              |
| Zuverlässigkeit        | Die Beliebtheit der App ist deutlich größer als gedacht. Statt 30 parallelen Nutzern, benutzen 500 Spieler die App. Kann die App noch immer jede Anfrage korrekt beantworten? | Nutzung  | Last-Tests                                                 |
|                        | Ein neuer Entwickler fügt eine schlecht programmierte Komponente hinzu, die Probleme verursacht. Die Änderung darf nicht auf main gelangen.                                   | Änderung | SonarCloud                                                 |
| Sicherheit             | Eine neue Komponente hat eine Sicherheitslücke. Sie darf nicht auf main gelangen                                                                                              | Änderung | SonarCloud                                                 |
|                        | Eine neue Sicherheitslücke wird bekannt, von der die App auch betroffen ist.                                                                                                  | Änderung | Trivy, CodeQL Scanner für C# und JS/TS                     |
| Benutzerfreundlichkeit | Das System beantwortet Anfragen auf Pokémons im Cache durchschnittlich in unter 30ms.<br>Auch bei 500 parallelen Anfragen soll dieses Ziel erreicht werden                    | Nutzung  | Last-Tests                                                 |
|                        | Nach einer Design-Änderung im Frontend kann die Website noch immer benutzt werden und liefert die korrekten Daten                                                             | Änderung | E2E-Test (Playwright)                                      |


# Risiken und technische Schulden

| Risiko / Technische Schuld | Beschreibung                                                                                                                                         | Maßnahme                                             | Prio   |
| -------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------- | ------ |
| Sicherheitslücken          | Sicherheitslücken können in Dependencies oder Docker Images erkannt werden.                                                                          | Regelmäßiger Dependency Scan, auch beim Docker Image | Hoch   |
| Belastung der Pokédex API  | Wird ein Pokémon, das nicht im Cache ist, gleichzeitig n mal abgerufen wird, wird die Pokédex API ]1..n\[ Mal aufgerufen.                            | Synchronisierung der API Anfragen                    | Hoch   |
| Performance im Load-Test   | Während dem Load Test wird die WebApp wie in einem Integration Test gestartet und läuft im gleichen Prozess. Dadurch ist die Performance schlechter. | Starten der WebApp in einem Docker Container         | Mittel |


# Glossar

| Begriff    | Definition                                                                 |
| ---------- | -------------------------------------------------------------------------- |
| C#         | Programmiersprache von Microsoft, verwendet u.a. bei .NET Applikationen    |
| ASP.NET    | Framework, mit dem C# Web Apps und APIs entwickelt werden können           |
| PostgreSQL | Relationale Datenbank                                                      |
| Komponente | Klasse, die einen eindeutig abgrenzbaren Anteil der Business Logic enthält |
