# Documentação da API — Beta Fit

Base URL (desenvolvimento): `https://localhost:5001`

Todas as respostas de erro seguem o formato:

```json
{
  "status": 400,
  "title": "Um ou mais campos são inválidos.",
  "errors": {
    "Name": ["O nome do produto é obrigatório."]
  }
}
```

## Categories

### `GET /api/categories?onlyActive={bool}`

Lista categorias. `onlyActive=true` retorna somente categorias ativas (usado pelo catálogo
público).

**Resposta 200:**
```json
[
  {
    "id": "guid",
    "name": "Camisetas",
    "description": "Camisetas leves e respiráveis.",
    "imageUrl": "/images/categories/camisetas.jpg",
    "isActive": true,
    "productCount": 2,
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": null
  }
]
```

### `GET /api/categories/{id}`
Retorna uma categoria pelo Id. `404` se não existir.

### `POST /api/categories`
**Body:** `CreateCategoryRequest { name, description, imageUrl }`
**Resposta:** `201 Created` com `CategoryResponse`.
**Erros:** `400` se o nome já existir ou for inválido.

### `PUT /api/categories/{id}`
**Body:** `UpdateCategoryRequest { name, description, imageUrl }`
**Resposta:** `200 OK` com `CategoryResponse`.

### `PATCH /api/categories/{id}/activate`
### `PATCH /api/categories/{id}/deactivate`
**Resposta:** `204 No Content`.

### `DELETE /api/categories/{id}`
**Resposta:** `204 No Content`.
**Erros:** `400` se a categoria possuir produtos vinculados.

---

## Products

### `GET /api/products`
Pesquisa com filtros, ordenação e paginação.

**Query params:**
| Nome | Tipo | Descrição |
|---|---|---|
| `searchTerm` | string | Busca em nome/descrição |
| `categoryId` | guid | Filtra por categoria |
| `gender` | enum (`Unissex`\|`Masculino`\|`Feminino`) | Filtra por gênero |
| `isActive` | bool | Filtra por status |
| `isFeatured` | bool | Filtra por destaque |
| `sortBy` | string (`price_asc`\|`price_desc`\|`newest`) | Ordenação; padrão é por nome |
| `page` | int | Página (padrão 1) |
| `pageSize` | int | Itens por página (padrão 12, máx. 100) |

**Resposta 200:** `PagedResponse<ProductListItemResponse>`
```json
{
  "items": [
    {
      "id": "guid",
      "name": "Camiseta Dry Performance",
      "price": 99.90,
      "imageUrl": "/images/products/camiseta-dry.jpg",
      "categoryName": "Camisetas",
      "gender": "Masculino",
      "isFeatured": true,
      "isActive": true
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 12,
  "totalPages": 2
}
```

### `GET /api/products/featured?take={int}`
Produtos em destaque para a Home (padrão `take=8`).

### `GET /api/products/{id}`
Detalhe completo (`ProductResponse`). `404` se não existir.

### `GET /api/products/{id}/related?take={int}`
Produtos da mesma categoria, excluindo o próprio produto (padrão `take=4`).

### `POST /api/products`
**Body:** `CreateProductRequest { name, description, price, imageUrl, categoryId, gender, isFeatured }`
**Resposta:** `201 Created` com `ProductResponse`.

### `PUT /api/products/{id}`
**Body:** `UpdateProductRequest { name, description, price, imageUrl, categoryId, gender }`
**Resposta:** `200 OK` com `ProductResponse`.

### `PATCH /api/products/{id}/activate`
### `PATCH /api/products/{id}/deactivate`
### `PATCH /api/products/{id}/featured?isFeatured={bool}`
**Resposta:** `204 No Content`.

### `DELETE /api/products/{id}`
**Resposta:** `204 No Content`.

---

## Dashboard

### `GET /api/dashboard/summary`
Métricas simples para a tela inicial do Desktop.

**Resposta 200:**
```json
{
  "totalProducts": 15,
  "activeProducts": 15,
  "featuredProducts": 5,
  "totalCategories": 9,
  "activeCategories": 9
}
```

---

## Enums

### Gender
| Valor | Descrição |
|---|---|
| `Unissex` | Produto sem restrição de gênero |
| `Masculino` | Linha masculina |
| `Feminino` | Linha feminina |

---

## Observações

- Não há autenticação nesta versão do projeto (escopo institucional/demonstrativo).
- Não há endpoints de pedido, pagamento, carrinho ou estoque — propositalmente fora do escopo.
- O botão "Tenho interesse" na página de produto do Website direciona para um link de WhatsApp
  fixo e demonstrativo, sem qualquer integração real.

## Segurança administrativa
Operações não-GET da API `/api/v1` exigem o header `X-Admin-Key`. Em produção, configure `Admin:ApiKey` por variável de ambiente/secret store. O valor `CHANGE_ME_DEV_ONLY` existe apenas para desenvolvimento local e deve ser substituído.

## Operação
- `GET /health` verifica disponibilidade da API.
- Endpoints públicos são somente leitura.
- Operações administrativas são protegidas por chave e devem ficar atrás de HTTPS.
- O Desktop envia a chave configurada em `BetaFitApi:AdminKey`.
- Nunca versionar chaves reais no repositório.
