JEUX DE TEST D'IMPORT BRIO — PRAXIS360

1. Brio-Test-Valide-Anonymise.csv
- 62 colonnes, séparateur point-virgule, encodage UTF-8 avec BOM.
- 3 clients fictifs.
- 5 contrats fictifs.
- 9 lignes de données.
- Contient plusieurs lignes pour un même contrat afin de tester le regroupement.
- Contient les compagnies Vivium, AG Insurance, Generali Belgium et NN Insurance Belgium.
- Generali et NN restent écrits comme dans la source : le mapping vers Athora doit être testé dans l'application, pas modifié dans le CSV source.
- Les colonnes Réserve et Capital sont présentes, comme dans l'export réel, mais ne doivent pas encore alimenter automatiquement les cartes financières tant que leur sens métier n'est pas validé.

2. Brio-Test-Erreurs-Anonymise.csv
Cas volontaires :
- numéro de contrat manquant ;
- numéro au format scientifique ;
- compagnie inconnue ;
- produit inconnu ;
- statut inconnu ;
- date invalide ;
- doublon exact.

CONFIDENTIALITÉ
- Tous les noms, coordonnées, identifiants, numéros de contrat et montants sont fictifs.
- Les fichiers peuvent être utilisés dans le projet et ajoutés au dépôt uniquement si l'équipe valide leur emplacement et leur usage de test.
- Le fichier BRIO réel ne doit jamais être ajouté au dépôt GitHub.
