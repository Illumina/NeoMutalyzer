# NeoMutalyzer

```
>dotnet NeoMutalyzer.dll Cache26_transcripts.tsv.gz Homo_sapiens.GRCh37.Nirvana.dat CosmicCodingMuts.json.gz
- loading reference sequences... 390 loaded.
- loading transcripts... 196,396 loaded.
1-877831-T-C    NM_152486.2     NM_152486.2:c.1027T>C   NM_152486.2:c.1027T>C(p.(Arg343=))      HGVS c. RefAllele
1-887801-A-G    NM_015658.3     NM_015658.3:c.1182T>C   NM_015658.3:c.1182T>C(p.(Thr394=))      HGVS c. RefAllele
1-888659-T-C    NM_015658.3     NM_015658.3:c.898A>G    NM_015658.3:c.898A>G(p.(Val300=))       HGVS c. RefAllele
1-1269554-T-C   NM_152228.1     NM_152228.1:c.2269T>C   NP_689414.1:p.(Cys757Arg)       HGVS c. RefAllele       HGVS p. RefAllele       cDNA RefAllele CDS RefAllele    Protein RefAllele
1-1269554-T-C   NM_152228.2     NM_152228.2:c.2269T>C   NM_152228.2:c.2269T>C(p.(Arg757=))      HGVS c. RefAllele
1-1288583-C-G   NM_001282585.1  NM_001282585.1:c.*98G>C         HGVS c. RefAllele
1-1288583-C-G   NM_001282584.1  NM_001282584.1:c.*401G>C                HGVS c. RefAllele
1-1288583-C-G   NM_032348.3     NM_032348.3:c.*401G>C           HGVS c. RefAllele
[...]
Y-1271289-A-T   NM_022148.2     NM_022148.2:c.466T>A    NP_071431.2:p.(Phe156Ile)       cDNA RefAllele
Y-1275371-C-T   NM_022148.2     NM_022148.2:c.304G>A    NP_071431.2:p.(Gly102Arg)       cDNA RefAllele
Y-1277731-T-G   NM_022148.2     NM_022148.2:c.150A>C    NP_071431.2:p.(Lys50Asn)        cDNA RefAllele

Transcript-Variants: bad: 125,029 / 37,525,892 (0.333%)
VIDs:                bad: 62,082 / 12,058,208 (0.515%)
Transcripts:         bad: 3,667 / 58,412 (6.278%)
Genes:               bad: 1,623 / 22,577 (7.189%)

  - elapsed time: 00:35:50.9
  - current RAM:  7.285 GB
  - peak RAM:     7.285 GB
```

| Category                 | Description                                                                                    | 
|--------------------------|------------------------------------------------------------------------------------------------| 
| HGVS c. RefAllele        | The reference bases in the HGVS notation don't match the transcript bases at the HGVS position | 
| HGVS c. Position         | The right aligned CDS coordinate doesn't match the position in the HGVS notation               | 
| HGVS c. CDS range        | The HGVS notation indicates that the coordinate is both before the coding region and after it  | 
| HGVS p. RefAllele        | The reference AA in the HGVS notation don't match the AAs at the HGVS position                 | 
| HGVS p. Unknown          | One of the ref AAs uses an unknown amino acid abbreviation (usually _?_)                       | 
| HGVS p. Position         | (not used at the moment)                                                                       | 
| HGVS p. AltAllele        | "This HGVS seems to be from our ""unknown"" output template."                                  | 
| cDNA RefAllele           | The reference bases in codons don't match the cDNA bases found at cdnaPos                      | 
| CDS RefAllele            | The reference bases in codons don't match the CDS bases found at cdsPos                        | 
| Invalid cDNA position    | "cDNA position beyond the bounds of the cDNA sequence [1, length]"                             | 
| Invalid CDS position     | "CDS position beyond the bounds of the CDS sequence [1, length]"                               | 
| Invalid protein position | "AA position beyond the bounds of the AA sequence [1, length]"                                 | 
| Protein RefAllele        | The reference AAs in aminoAcids don't match the AAs found at proteinPos                        | 
