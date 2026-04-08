# Frontend — poznámky k implementaci

Tento dokument doplňuje `spec.md` o rozhodnutí při implementaci a body vhodné pro code review nebo pohovor.

## Autentizace: `X-Api-Key` versus JWT login

Ve specifikaci je popsána autentizace pomocí API klíče a hlavičky `X-Api-Key`. **Po vzájemné domluvě s backendem / hodnotiteli** lze stejný záměr (nejprve ověřit klienta, pak volat endpointy úkolů) řešit **přihlášením a JWT**: výměna přihlašovacích údajů za token a posílání tokenu u dalších požadavků (např. `Authorization: Bearer`). Klient pak nepoužívá surový API klíč v hlavičce `X-Api-Key`, ale výsledné chování z hlediska „nejprve ověření, pak aplikace“ je obdobné.

## Globální ošetření chyb

Aplikace používá **globální `ErrorHandler`**, který mapuje HTTP **401** a **500** na srozumitelné texty a nabízí jednu akci obnovy.

**Záměr (obdobně nízkoúrovňovému selhání):** jde o analogii k chování typu **kernel panic**: **blokující, jednoduché hlášení**, které nepředstírá, že je bezpečné pokračovat v částečně rozbitém stavu, a **`reload`** jako explicitní cesta obnovy po selhání autentizace nebo serveru. Za cenu jemnější UX se volí **předvídatelnost** a **jasná obnova**.

**Upozornění:** mnoho **HTTP chyb ošetřených uvnitř `subscribe({ error: … })`** se do globálního `ErrorHandler` v Angularu **typicky nedostane**. Globální handler tedy pokrývá hlavně **neošetřené** chyby a situace, které projdou mechanismem zón / chyb; chyby API řešené lokálně v komponentách mohou vyžadovat **duplicitní** nebo **centralizované** řešení (např. HTTP interceptor + notifikační službu), pokud má být každé selhání sjednocené.

## Responzivita

Seznam úkolů používá **kontejner tabulky s horizontálním posuvem** (`.table-scroll` s `overflow-x`) a **pružný toolbar**, aby rozložení zůstalo použitelné na úzkých displejích. Zadání vyžaduje **použitelnost** na desktopu i mobilu, ne konkrétní vzor; alternativa v podobě **karet** na malé obrazovce v zadání není a šlo by ji doplnit později.
