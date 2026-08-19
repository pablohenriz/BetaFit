# Beta Fit — funcionalidades adicionadas

## Login e cadastro
- Cadastro de cliente em `/Register`.
- Login em `/Login`.
- Autenticação via JWT na API e cookie na UI.
- Perfil `Customer` para usuários comuns.
- Perfil `Admin` para administração.

## Admin
Acesse `/Admin` com:
- e-mail: `admin@betafit.local`
- senha: `Admin123!`

O administrador pode:
- ver os produtos cadastrados;
- adicionar produtos;
- enviar imagem JPG, PNG ou WEBP;
- escolher categoria/gênero;
- marcar destaque.

## Carrinho
O carrinho é propositalmente simples e funciona em sessão:
- adicionar produto;
- aumentar quantidade ao adicionar novamente;
- remover produto;
- calcular total.

Não existe checkout, pagamento ou pedido real.

## Banco
O projeto foi ajustado para `EnsureCreated` para facilitar a execução escolar em um banco novo. Se você já tiver um banco antigo criado antes destas alterações, apague o banco `BetafitDb` e execute novamente para que as tabelas de usuários sejam criadas.

## Execução
1. Inicie `BetaFit.API`.
2. Inicie `BetaFit.UI`.
3. Abra a UI e faça cadastro/login.
