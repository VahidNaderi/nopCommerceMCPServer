NopPlus.Mcp.Core

Lightweight MCP framework for nopCommerce plugins. Responsibilities:

- Tool abstraction and registry
- Middleware-style execution pipeline (authentication, authorization, audit)
- JSON request/response handling
- Endpoint registration extension for consumer plugins

Usage:

1. In Consumer plugin's ConfigureServices: services.AddMcpCore();
2. Register IAuthenticationProvider, IAuthorizationHandler and any middlewares or tools (IToolRegistry.Register)
3. In Configure: app.UseMcpEndpoint("/mcp");

Design notes: keep this project free of business tools. Consumer plugins implement and register IMcpTool instances.
