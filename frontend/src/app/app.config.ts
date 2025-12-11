import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptorFn } from './interceptors/auth.interceptor';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';

export const appConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptorFn])
    )
  ]
};
