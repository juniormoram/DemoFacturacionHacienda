Demo Facturación Electrónica Costa Rica

Sistema demo de facturación electrónica desarrollado en .NET (ASP.NET Core Web API) que simula el flujo completo requerido por el Ministerio de Hacienda de Costa Rica, incluyendo generación de XML, firma (simulada), envío y consulta de estado.

Este proyecto fue desarrollado como demostración para:

Comprender el flujo real de facturación electrónica en Costa Rica Integrar servicios externos (Hacienda) Manejar generación de XML estructurado Simular procesos de firma digital

Flujo de facturación electrónica:

Se crea una factura desde el API
Se genera el XML conforme al formato requerido
Se aplica firma digital (simulada)
Se envía a Hacienda (sandbox)
Se consulta el estado del comprobante
Se actualiza el estado en base de datos
El sistema implementa: Autenticación con API de Hacienda (sandbox) Envío de comprobantes electrónicos Consulta de estado de documentos

La firma digital requerida (XAdES-EPES) por el Ministerio de Hacienda de Costa Rica no se implementa completamente en este proyecto.

En su lugar: Se utiliza una firma simulada La arquitectura está preparada para integrar firma real mediante certificados (.p12)
