import { Request, Response, NextFunction } from "express";

/**
 * Gelen her HTTP isteğini ve dönen yanıtı loglar.
 * Method, URL, status code, süre ve varsa hata bilgisi gösterilir.
 */
export function requestLogger(
  req: Request,
  res: Response,
  next: NextFunction
): void {
  const start = Date.now();

  // Query params varsa logla
  const query = Object.keys(req.query).length
    ? ` query=${JSON.stringify(req.query)}`
    : "";

  console.log(`→ ${req.method} ${req.originalUrl}${query}`);

  // Yanıt bittiğinde logla
  res.on("finish", () => {
    const duration = Date.now() - start;
    const status = res.statusCode;
    const level = status >= 400 ? "⚠" : "✓";
    console.log(`${level} ${req.method} ${req.originalUrl} → ${status} (${duration}ms)`);
  });

  next();
}
