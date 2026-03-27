Demo Facturación Electrónica Costa Rica

Demo en vivo: https://demo-facturacion-hacienda.onrender.com
⚠️ Hosted en Render (plan gratuito) — puede tardar unos minutos en cargar si el servidor estuvo inactivo.

Sistema demo de facturación electrónica desarrollado en .NET (ASP.NET Core Web API) que simula el flujo completo requerido por el Ministerio de Hacienda de Costa Rica, incluyendo generación de XML, firma (simulada), envío y consulta de estado.

Este proyecto fue desarrollado como demostración para:
- Comprender el flujo real de facturación electrónica en Costa Rica
- Integrar servicios externos (Hacienda)
- Manejar generación de XML estructurado
- Simular procesos de firma digital

Flujo de facturación electrónica:

1. Se crea una factura desde el API
2. Se genera el XML conforme al formato requerido
3. Se aplica firma digital (simulada)
4. Se envía a Hacienda (sandbox)
5. Se consulta el estado del comprobante
6. Se actualiza el estado en base de datos

El sistema implementa: 
- Autenticación con API de Hacienda (sandbox) 
- Envío de comprobantes electrónicos 
- Consulta de estado de documentos

La firma digital requerida (XAdES-EPES) por el Ministerio de Hacienda de Costa Rica no se implementa completamente en este proyecto.

En su lugar, se utiliza una firma simulada.
La arquitectura está preparada para integrar firma real mediante certificados (.p12)
