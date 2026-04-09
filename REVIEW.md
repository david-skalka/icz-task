# Hodnocení zpracování

## Pozitivní

- velmi kladně hodnotím použítí [JwtOptions.cs](Api/IczTask/Options/JwtOptions.cs) toto je správný postup pro konfiguraci v produkčních aplikacích. Jediná připomínka (nikoliv výtka) by byla použití `const stringu` v rámci třídy, který označuje danou sekci v `application.json` viz [JwtOptions.cs](Api/IczTask/Options/JwtOptions.cs)
- Velmi dobré použítí migrace v rámci `Program.cs`, kde se ověří zda běží aplikace ve správném prostředí a pustí se migrace. U nás si toto většinou řešíme jinak, ale velmi cením tento přístup!

## Neutrální

- Použití Hybrid cache dobré, jediná věc ohledně konfigurace. V produkčním prostředí by se použila nějaká možnost konfigurace, abychom kvůli cache nemuseli releasovat novou verzi aplikace. V tomto zadání to ale není špatně.
- Použití Hybrid Cache v rámci [TaskController.cs](Api/IczTask/Controllers/TaskController.cs) nebude fugnovat. Pokud chci vytvořit nový úkol tak smažu všechny úkoly z cache.
- Jenom takový TIP, pokud používáme EF Core tak ten má v sobě zabudovaný change tracking, který když uděláme `db.SaveChangesAsync();` tak aktualizuje naše entity. Viz [TaskController.cs](Api/IczTask/Controllers/TaskController.cs)

## Závěr
Vše v pořádku, žádné problémy jsem nenašel, jenom drobnosti.