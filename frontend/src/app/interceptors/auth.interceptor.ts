import { inject } from '@angular/core';
import { HttpRequest, HttpHandlerFn, HttpEvent } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';

export const authInterceptorFn = (
  req: HttpRequest<unknown>, 
  next: HttpHandlerFn
) : Observable<HttpEvent<unknown>> => {
  
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (token && req.url.startsWith(environment.API_URL)) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req).pipe(
    tap({
      next: (event) => console.log('Response received:', event),
      error: (error) => console.error('Error request:', error)
    })
  );
};