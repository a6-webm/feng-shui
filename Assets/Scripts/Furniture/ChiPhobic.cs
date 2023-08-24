using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiPhobic : FurnRestriction {

    private int _chiColliders = 0;

    override protected bool lockingCondition() {
        return _chiColliders != 0;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Chi") {
            _chiColliders++;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.tag == "Chi") {
            _chiColliders--;
        }
    }
}
