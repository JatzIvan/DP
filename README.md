# DP - Ivan Jatz

## Štart aplikácie - bližšie informácie v projekte traffic-dt-deployment folder

## Opis projektov

### ApiLibrary

Knižnica na REST požiadavky (čisto len lib)

### CollisionDetector

Spustiteľná aplikácia - pomocou ``dotnet run``. Taktiež obsahuje ``Dockerfile`` na vytvorenie obrazu. Ten je využívaný pri riadenom nasadení projektom traffic-dt-deployment

### CoreLibrary

Knižnica obsahujúca väčšinu implementácie (čisto len lib)

### RoadVisualisationWeb

Pomocný projekt na vizualizáciu cesty pred nasadením. Primárne sa využíva na úpravu parametrov. Spustenie pomocou ``dotnet run``. Web sa nachádza na adrese http://localhost:5164/

### SimpleLogger

Knižnica obsahujúca nástroje na logovanie (čisto len lib)

### SumoTraceParser

Pomocná knižnica použitá na záťažové testovanie aplikácie (spustiteľná)

### TestingLibrary

Pomocná knižnica použitá na jednotkové testovanie (automaticky spustená cez CI/CD)

### WebSocketLibrary

Knižnica obsahujúca celú implementáciu UDP komunikácie cez sockety (čisto len lib)
