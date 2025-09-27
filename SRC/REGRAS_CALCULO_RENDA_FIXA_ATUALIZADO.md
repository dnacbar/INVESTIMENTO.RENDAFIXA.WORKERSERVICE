# Guia Atualizado de Regras de Cálculo de Renda Fixa no Brasil

## 1. Instrumentos Suportados
- CDB, LC, LCI, LCA, Debêntures
- Tesouro Direto (Selic, IPCA+, Prefixado)
- Fundos DI / Renda Fixa

## 2. Tipos de Remuneração
- **Prefixado**: taxa fixa ao ano (ex.: 10% a.a.).
- **Pós-fixado**: atrelado ao CDI/SELIC (% do CDI, etc.).
- **Híbrido (IPCA + Taxa Real)**.

## 3. Dados Necessários
- Valor aplicado (**principal**)
- Datas de aplicação e resgate
- Tipo de título e indexador
- Taxa anual ou percentual do CDI
- Taxas administrativas (se houver)
- Convenção de dias (252 úteis, 365 corridos, configurável)

## 4. Fórmulas Principais

### Prefixado
```
valor_final = principal * (1 + taxa_anual)^(dias / convencao)
```

### CDI (% do CDI)
```
cdi_diario = (1 + CDI_a.a.)^(1/convenção) - 1
rendimento_diario_produto = cdi_diario * (percentual / 100)
valor_final = principal * (1 + rendimento_diario_produto)^dias
```

### IPCA+
```
valor_final = principal * (1 + IPCA_acumulado) * (1 + taxa_real)^(períodos)
```

## 5. IOF
- Incide apenas sobre o **rendimento**, em resgates antes de 30 dias.
- **Tabela regressiva** (configurável):
  - Dia 1: 96%
  - Dia 2: 93%
  - ...
  - Dia 29: 3%
  - Dia 30 em diante: 0%

### Cálculo:
```
iof = rendimento_bruto * percentual_iof[dias]
```

## 6. IR (Imposto de Renda Retido na Fonte)
- Incide sobre o rendimento líquido de IOF.
- **Tabela regressiva** (configurável):
  - Até 180 dias: 22,5%
  - 181 a 360 dias: 20%
  - 361 a 720 dias: 17,5%
  - Acima de 720 dias: 15%

### Cálculo:
```
ir = (rendimento_bruto - iof) * aliquota_ir
```

## 7. Taxas Administrativas
- Deduzidas após impostos, se houver.
- Devem ser informadas e consideradas no cálculo do valor líquido.

## 8. Isenções
- LCI e LCA: isentas de IR para pessoa física (regra atual).
- Outras exceções podem ser definidas por legislação.

## 9. Ordem de Cálculo
1. Calcular número de dias (úteis ou corridos, conforme convenção).
2. Calcular rendimento bruto (segundo indexador e tipo de remuneração).
3. Aplicar IOF (se <30 dias).
4. Aplicar IR (sobre rendimento - IOF).
5. Deduzir taxas administrativas (se houver).
6. Obter valor líquido final.
7. Gerar breakdown detalhado (bruto, IOF, IR, taxas, líquido).

## 10. Exemplo Numérico
- **Principal**: R$10.000
- **Taxa Prefixada**: 10% a.a.
- **Prazo**: 200 dias úteis

### Passos:
1. Valor bruto = `10.000 * (1+0.10)^(200/252) = 10.785,77`
2. Rendimento bruto = `785,77`
3. IOF = `0` (pois >30 dias)
4. IR = `785,77 * 20% = 157,15`
5. Taxa administrativa = `0` (exemplo)
6. Rendimento líquido = `628,62`
7. Valor final líquido = `10.628,62`

## 11. Boas Práticas
- Manter tabelas IOF e IR **configuráveis e atualizadas**.
- Suportar convenções de dias diferentes.
- Exibir **breakdown detalhado** (bruto, IOF, IR, taxas, líquido).
- Validar resultados com simuladores oficiais (Tesouro Direto, ANBIMA, bancos).
- Implementar testes automatizados para todos os cenários de cálculo.
