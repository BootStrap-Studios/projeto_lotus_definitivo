using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave
{
    void CarregarSave(InfosSave save);

    void SalvarSave(ref InfosSave save);
}
