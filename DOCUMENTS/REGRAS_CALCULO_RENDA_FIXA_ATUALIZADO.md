# Guia Atualizado de Regras de C�lculo de Renda Fixa no Brasil

## 1. Instrumentos Suportados
- CDB, LC, LCI, LCA, Deb�ntures
- Tesouro Direto (Selic, IPCA+, Prefixado)
- Fundos DI / Renda Fixa

## 2. Tipos de Remunera��o
- **Prefixado**: taxa fixa ao ano (ex.: 10% a.a.).
- **P�s-fixado**: atrelado ao CDI/SELIC (% do CDI, etc.).
- **H�brido (IPCA + Taxa Real)**.

## 3. Dados Necess�rios
- Valor aplicado (**principal**)
- Datas de aplica��o e resgate
- Tipo de t�tulo e indexador
- Taxa anual ou percentual do CDI
- Taxas administrativas (se houver)
- Conven��o de dias (252 �teis, 365 corridos, configur�vel)

## 4. F�rmulas Principais

### Prefixado
```
valor_final = principal * (1 + taxa_anual)^(dias / convencao)
```

### CDI (% do CDI)
```
cdi_diario = (1 + CDI_a.a.)^(1/conven��o) - 1
rendimento_diario_produto = cdi_diario * (percentual / 100)
valor_final = principal * (1 + rendimento_diario_produto)^dias
```

### IPCA+
```
valor_final = principal * (1 + IPCA_acumulado) * (1 + taxa_real)^(per�odos)
```

## 5. IOF
- Incide apenas sobre o **rendimento**, em resgates antes de 30 dias.
- **Tabela regressiva** (configur�vel):
  - Dia 1: 96%
  - Dia 2: 93%
  - ...
  - Dia 29: 3%
  - Dia 30 em diante: 0%

### C�lculo:
```
iof = rendimento_bruto * percentual_iof[dias]
```

## 6. IR (Imposto de Renda Retido na Fonte)
- Incide sobre o rendimento l�quido de IOF.
- **Tabela regressiva** (configur�vel):
  - At� 180 dias: 22,5%
  - 181 a 360 dias: 20%
  - 361 a 720 dias: 17,5%
  - Acima de 720 dias: 15%

### C�lculo:
```
ir = (rendimento_bruto - iof) * aliquota_ir
```

## 7. Taxas Administrativas
- Deduzidas ap�s impostos, se houver.
- Devem ser informadas e consideradas no c�lculo do valor l�quido.

## 8. Isen��es
- LCI e LCA: isentas de IR para pessoa f�sica (regra atual).
- Outras exce��es podem ser definidas por legisla��o.

## 9. Ordem de C�lculo
1. Calcular n�mero de dias (�teis ou corridos, conforme conven��o).
2. Calcular rendimento bruto (segundo indexador e tipo de remunera��o).
3. Aplicar IOF (se <30 dias).
4. Aplicar IR (sobre rendimento - IOF).
5. Deduzir taxas administrativas (se houver).
6. Obter valor l�quido final.
7. Gerar breakdown detalhado (bruto, IOF, IR, taxas, l�quido).

## 10. Exemplo Num�rico
- **Principal**: R$10.000
- **Taxa Prefixada**: 10% a.a.
- **Prazo**: 200 dias �teis

### Passos:
1. Valor bruto = `10.000 * (1+0.10)^(200/252) = 10.785,77`
2. Rendimento bruto = `785,77`
3. IOF = `0` (pois >30 dias)
4. IR = `785,77 * 20% = 157,15`
5. Taxa administrativa = `0` (exemplo)
6. Rendimento l�quido = `628,62`
7. Valor final l�quido = `10.628,62`

## 11. Boas Pr�ticas
- Manter tabelas IOF e IR **configur�veis e atualizadas**.
- Suportar conven��es de dias diferentes.
- Exibir **breakdown detalhado** (bruto, IOF, IR, taxas, l�quido).
- Validar resultados com simuladores oficiais (Tesouro Direto, ANBIMA, bancos).
- Implementar testes automatizados para todos os cen�rios de c�lculo.
