# API — poznámky k implementaci

Tento dokument doplňuje `spec.md` o rozhodnutí při implementaci a kontext vhodný pro code review nebo pohovor.

## Verze .NET (9 vs 10)

V zadání je v jednom místě uvedeno **.NET 10** (nebo novější) a v sekci o Hybrid Cache nadpis **„(.NET 9)“**, takže dokument sám o sobě působí nekonzistentně. **.NET 10** v době řešení nebyl v prostředí zprovozněný a z časových důvodů byla zvolena cílová verze **.NET 9**, na které je projekt postavený a testovaný.

## Autentizace: JWT místo X-Api-Key

Technické zadání počítá s **API klíčem** v hlavičce **`X-Api-Key`** a uložením v `appsettings.json`. **Na pohovoru bylo domluveno**, že je **přípustné** stejný záměr (ověření klienta před přístupem k endpointům úkolů) řešit **JWT** (např. `Authorization: Bearer`) místo `X-Api-Key`. Implementace proto používá JWT v souladu s touto domluvou, nikoli doslovně text zadání u API klíče.
