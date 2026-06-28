import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function passwordMatch(controlName: string, matchingControlName: string): ValidatorFn {
  return (formGroup: AbstractControl): ValidationErrors | null => {
    const control = formGroup.get(controlName);
    const matchingControl = formGroup.get(matchingControlName);

    if (!control || !matchingControl) return null;

    if (matchingControl.errors && !matchingControl.errors['passwordMismatch']) {
      return null;
    }

    if (control.value !== matchingControl.value) {
      matchingControl.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      matchingControl.setErrors(null);
      return null;
    }
  };
}

export function telefoneValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const val = control.value.replace(/\s+/g, '');
    const regex = /^(\+244)?9\d{8}$/;
    return regex.test(val) ? null : { telefoneInvalido: true };
  };
}

export function biValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const val = control.value.trim().toUpperCase();
    const regex = /^\d{9}[A-Z]{2}\d{3}$/;
    return regex.test(val) ? null : { biInvalido: true };
  };
}

export function dataNaoFuturaValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const data = new Date(control.value);
    if (isNaN(data.getTime())) return null;
    const hoje = new Date();
    hoje.setHours(23, 59, 59, 999);
    return data > hoje ? { dataFutura: true } : null;
  };
}

export function idadeMinimaValidator(idadeMinima: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const data = new Date(control.value);
    if (isNaN(data.getTime())) return null;
    const hoje = new Date();
    const idade = hoje.getFullYear() - data.getFullYear();
    const mes = hoje.getMonth() - data.getMonth();
    const dia = hoje.getDate() - data.getDate();
    const idadeReal = mes < 0 || (mes === 0 && dia < 0) ? idade - 1 : idade;
    return idadeReal < idadeMinima ? { idadeMinima: { required: idadeMinima, actual: idadeReal } } : null;
  };
}
