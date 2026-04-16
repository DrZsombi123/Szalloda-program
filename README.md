# Szálloda Nyilvántartó

C# konzolos szálloda-foglalás nyilvántartó program.

## Funkciók
- Foglalások listázása, felvétele, módosítása, törlése
- Keresés vendégnév alapján
- Szabad szobák keresése megadott időszakra
- Automatikus ár kalkuláció (éjszakák × ár/éj)
- Szoba-ütközés detektálása (nincs dupla foglalás!)
- CSV fájl perzisztencia

## Ütemterv
1.NAP (ma) — 5 commit
    Zsombor: COMMIT 1 (projekt + Foglalas.cs) 
    Gergő: COMMIT 2 (menü váz)
    Gergő: COMMIT 3 (listázás)
    Gergő: COMMIT 4 (új foglalás input)
    Zsombor: COMMIT 5 (CSV mentés)

2.NAP — 6 commit
    Zsombor: COMMIT 6 (CSV betöltés)
    Gergő: COMMIT 7 (validáció)
    Gergő: COMMIT 8 (módosítás)
    Zsombor: COMMIT 9 (törlés)
    Gergő: COMMIT 10 (ütközés-detektálás)
    Gergő: COMMIT 11 (keresés név szerint)

3.NAP — 4 commit + teszt
    Zsombor: COMMIT 12 (szabad szobák)
    Gergő: COMMIT 13 (try-catch)
    Zsombor: COMMIT 14 (formázás)
    Gergő: COMMIT 15 (README + minta adatok)
    Teljes tesztelés
    Prezentáció begyakorlása

## Futtatás
```
dotnet run
```

## Készítették
- [Diák1 neve] - fájlkezelés, adat-osztály, törlés, szabad szobák keresése
- [Diák2 neve] - menü rendszer, CRUD, validáció, ütközés-ellenőrzés, keresés